'use strict';

/**
 * @ngdoc function
 * @name initProjApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the initProjApp
 */
 var keys = {}, dragId = -1, dragParent = {}, zIndex = 5000; // evil global and drag flag

angular.module('initProjApp').controller('WorkspaceCtrl', function ($scope, storage) {

	$scope.resc = {
		"cpyChd": "Copy Children",
		"ascFile": "Associate File",
		"filesOk": "All files are OK!",
		"filesChg": "A file has changed since last test!",
		"filesRmv": "A file has been removed since last test!",
		"tfs": "TFS",
		"localStore": "Local",
		"pasteSteps": "Paste Steps"
	};
  	storage.bind($scope,'entries');
  	storage.bind($scope,'idStore');

  	$scope.disabled = false;
	$scope.canEdit = true;

	$scope.search_box = "";
	
	if(!$scope.idStore || $scope.idStore <= 0)
		$scope.idStore = 1;

	$scope.getEntryTemplate = function () { return {
		"id" : 0,
		"name" : "",
		"type" : "",
		"parent" : 0,
		"parentIndex" : -1,
		"size" : "",
		"tags" : [],
		"result" : "",
		"toggle" : false,
		"toggleDetails" : false,
		"summary" : "",
		"store" : ["Local"],
		"children" : [],
		"suites" : []
	}; };

	$scope.createId = function() {
		$scope.idStore++;
		return $scope.idStore;
	}

	$scope.timeOut = 0;

  	$scope.viewType = 'ANYTHING';

  	if(!$scope.types || $scope.types.length < 3)
		$scope.types = ['step', 'case', 'suite'];

	if(!$scope.entries || $scope.entries.length <= 0) {
	    $scope.entries = [];
	    var tmp = $scope.getEntryTemplate();
	    tmp.id = $scope.createId();
	    tmp.name = "New Test Suite";
	    tmp.type = "suite";
	    tmp. size = sizeByType("suite");
	    addEntry(tmp);
	}

	$scope.copyBuffer = false;
	// currently active suite
	$scope.active = { "index" : 0 };
	$scope.suite = $scope.entries[0];

	$scope.clearStorage = function() {
		storage.clearAll();
		storage.bind($scope,'entries');
		$scope.entries = [];
		$scope.addSuite();
	}

	$scope.copySteps = function(index) {
		$scope.copyBuffer = index;
	}

	$scope.pasteSteps = function(index) {
		if($scope.copyBuffer === false)
			return;
		var len = $scope.suite.children[index].children.length;
		var tmp = $scope.suite.children[$scope.copyBuffer].children.deepClone();
		// alter step parents, also attempt to give uniqueness to object, else repeater will fail.
		tmp = tmp.map(function(x) { 
			x.name += " >> from " + $scope.suite.children[$scope.copyBuffer].name; 
			x.id = $scope.createId(); 
			x.parent = $scope.suite.children[index].id; 
			return x; 
		});
		var len = $scope.suite.children[index].children.length;
		if(len > 0) {
			$scope.suite.children[index].children = $scope.suite.children[index].children.concat(tmp);
		}
		else
			$scope.suite.children[index].children = tmp;
		if(len < $scope.suite.children[index].children.length) {
			$scope.copyBuffer = false;
			toastr.success('Copy Success!', 'The children were copied successfully.');
		} else {
			toastr.error('Copy Error', 'Copy operation was not successful, entries still in buffer.');
		}
	}

	$scope.hasCopyBuffer = function() {
		return $scope.copyBuffer !== false;
	}

	$scope.preventDefault = function(event) {
		event.preventDefault();
	};

	$scope.makeActive = function(index, parent, root) {
		$scope.active = {"root": -1, "parent": -1, "index" : -1};
		if(typeof parent === 'undefined' && typeof root === 'undefined') {
			$scope.active.index = index;
			$scope.suite = $scope.entries[index];
		} else if(typeof root === 'undefined') {
			$scope.active.parent = parent;
			$scope.active.index = index;
			$scope.suite = $scope.entries[parent].suites[index];
		} else {
			$scope.active.root = root;
			$scope.active.parent = parent;
			$scope.active.index = index;
			$scope.suite = $scope.entries[root].suites[parent].suites[index];
		}
	}

	$scope.isActive = function(id) {
		if(id == $scope.suite.id)
			return "active";
	}

	/* going to move
	$scope.addStore = function(index, storeName) {
		var id = $.inArray(storeName, $scope.entries[index].store);
		if(id < 0)
			$scope.entries[index].store.push(storeName);
	}

	$scope.hasStore = function(index, storeName) {
		if($.inArray(storeName, $scope.entries[index].store) >= 0)
			return true;
		return false;
	}

	$scope.removeStore = function(index, storeName) {
		var id = $.inArray(storeName, $scope.entries[index].store);
		if(id >= 0)
			$scope.entries[index].store.splice(id, 1);
	}

	$scope.toggleStore = function(index, storeName) {
		if($scope.hasStore(index, storeName))
			$scope.removeStore(index, storeName);
		else
			$scope.addStore(index, storeName);
	}

	end move */
	$scope.addSuite = function(name, parent, root) {
		if(typeof name === "undefined" && $("#newSuiteText").val().length <= 0)
			return;
		var tmp = $scope.getEntryTemplate();
	    tmp.id = $scope.createId();
	    if(typeof name === "undefined" && $("#newSuiteText").val().length > 0) {
	    	tmp.name = $("#newSuiteText").val();
	    	$("#newSuiteText").val("");
	    } else
			tmp.name = "New Suite";
	    tmp.type = "suite";
	    tmp. size = sizeByType("suite");
	    if(typeof parent === "undefined") {
	    	addEntry(tmp);
	    	$scope.makeActive(0);
	    } else if(typeof root === "undefined") {
	    	addEntry(tmp, parent);
	    	if($scope.isSuiteToggle(parent) != "down")
	    		$scope.toggleSuite(parent);
	    	$scope.makeActive(0, parent);
	    } else {
	    	addEntry(tmp, parent, root);
	    	if($scope.isSuiteToggle(parent, root) != "down")
	    		$scope.toggleSuite(parent, root);
	    	$scope.makeActive(0, parent, root);
	    }
	}

	$scope.toggle = function(index, force) {
		if(typeof force === "undefined") {
			$scope.suite.children[index].toggle = !$scope.suite.children[index].toggle;
			if($scope.suite.children[index].toggle && $scope.suite.children[index].children.length <= 0)
				addSubEntry(0, index);
		}
		else
			$scope.suite.children[index].toggle = true;
	}

	$scope.toggleDetails = function(index) {
		$scope.suite.children[index].toggleDetails = !$scope.suite.children[index].toggleDetails;
	}

	$scope.toggleClass = function(index) {
		if($scope.suite.children[index].toggle)
			return "test-expand";
		else
			return "test-in";
	}

	$scope.toggleDetailsClass = function(index) {
		if($scope.suite.children[index].toggleDetails)
			return " summary-expand";
		else
			return "";
	}

	$scope.toggleButton = function(index) {
		if($scope.suite.children[index].toggle)
			return "glyphicon-chevron-up";
		else
			return "glyphicon-chevron-down";
	}

	$scope.toggleDetailsButton = function(index) {
		if($scope.suite.children[index].toggleDetails)
			return "glyphicon-chevron-up";
		else
			return "glyphicon-chevron-down";
	}

	$scope.isLast = function(index, arr, css) {
		//console.log(index + " , " + arr + " , " + css);
		if(index >= arr.length - 1)
			return css;
		else
			return "";
	};

	$scope.isFirst = function(index, css) {
		if(index <= 0)
			return css;
		else
			return "";
	}

    $scope.addCaseClicked = function() {
    	addSubEntry(0);
    };

    $scope.addStepClicked = function(index) {
    	addSubEntry(0, index);
    };

    $scope.removeEntryClicked = function(index) {
    	$scope.entries.splice(index, 1);
    	$scope.makeActive(0);
    };

    $scope.removeCaseEntryClicked = function(index) {
    	$scope.suite.children.splice(index, 1);
    }

    $scope.removeStepEntryClicked = function(parent, index) {
    	$scope.suite.children[parent].children.splice(index, 1);
    }

    $scope.saveEntry = function() {
    	clearTimeout($scope.timeOut);
    	$scope.timeOut = setTimeout(function() {
    	}, 1000);
    }

	function addChild(parentId, child) {
		for(var i = 0, j = $scope.entries.length; i < j; i++) {
			if($scope.entries[i].id >= parentId)
				$scope.entries[i].children.push(child);
		}
	}

	function addEntry(entry, parent, root) {
		if(typeof parent === "undefined")
			$scope.entries.splice(0, 0, entry);
		else if(typeof root === "undefined")
			$scope.entries[parent].suites.splice(0, 0, entry);
		else
			$scope.entries[root].suites[parent].suites.splice(0, 0, entry);
	};

	function addSubEntry(sibling, index) {
		var tmp = $scope.getEntryTemplate();
		tmp.id = $scope.createId();
		if(typeof index === "undefined") {
			var type = $scope.typeDownOne($scope.suite.type);
		    tmp.type = type;
		    tmp.size = sizeByType(type);
		    tmp.parent = $scope.suite.id;
		    if($scope.suite.children.length > 0 && sibling >= 0 && sibling < $scope.suite.children.length)
				$scope.suite.children.splice(sibling, 0, tmp);
			else
				$scope.suite.children.push(tmp);
		} else {
			var type = $scope.typeDownOne($scope.suite.children[index].type);
		    tmp.type = type;
		    tmp.size = sizeByType(type);
		    tmp.parent = $scope.suite.children[index].id;
		    $scope.toggle(index, true);
		    if($scope.suite.children[index].children.length > 0 && sibling >= 0 && sibling < $scope.suite.children[index].children.length)
				$scope.suite.children[index].children.splice(sibling, 0, tmp);
			else
				$scope.suite.children[index].children.push(tmp);
		}
		return tmp.id;
	};


	$scope.typeDownOne = function(parentType) {
		if(typeof parentType === 'string')
			parentType = $scope.types.indexOf(parentType);

		if(parentType < $scope.types.length && parentType > 0)
			return $scope.types[parentType - 1];
		else
			return $scope.types[parentType];
	};

	$scope.typeUpOne = function(parentType) {
		if(typeof parentType === 'string')
			parentType = $scope.types.indexOf(parentType);

		if(parentType < ($scope.types.length - 1) && parentType >= 0)
			return $scope.types[parentType + 1];
		else
			return $scope.types[parentType];
	};

	$scope.nestSuite = function (index, parent)
	{
		if(index <= 0)
			return;

		if(typeof parent === 'undefined' && $scope.entries[index].suites.length <= 0) {
			$scope.entries[index].parent = $scope.entries[index - 1].id;
			$scope.entries[index].parentIndex = index - 1;
			$scope.entries[index - 1].suites.push($scope.entries[index]);
			$scope.entries.splice(index, 1);
		} else if($scope.entries[parent].suites[index].suites.length <= 0) { // do not nest to bottom tier if there are children
			$scope.entries[parent].suites[index].parent = $scope.entries[parent].suites[index - 1].id; 
			$scope.entries[parent].suites[index].parentIndex = index - 1;
			$scope.entries[parent].suites[index - 1].suites.push($scope.entries[parent].suites[index]);
			$scope.entries[parent].suites.splice(index, 1);
		}
	};

	$scope.unNestSuite = function (index, parent, root)
	{
		if(index < 0)
			return;
		if(typeof root === 'undefined') {
			$scope.entries[parent].suites[index].parent = 0
			$scope.entries[parent].suites[index].parentIndex = -1;
			$scope.entries.push($scope.entries[parent].suites[index]);
			$scope.entries[parent].suites.splice(index, 1);
		} else {
			$scope.entries[root].suites[parent].suites[index].parent = $scope.entries[root].id;
			$scope.entries[root].suites[parent].suites[index].parentIndex = root;
			$scope.entries[root].suites.push($scope.entries[root].suites[parent].suites[index]);
			$scope.entries[root].suites[parent].suites.splice(index, 1);
		}
	};

	$scope.isSuiteToggle = function(index, parent) {
		if(index < 0)
			return;
		var toggle = false;
		if(typeof parent === 'undefined')
			toggle = $scope.entries[index].toggle;
		else
			toggle = $scope.entries[parent].suites[index].toggle;

		if(toggle)
			return "down";
		else
			return "up";
	};

	$scope.toggleSuite = function(index, parent) {
		if(index < 0)
			return;

		if(typeof parent === 'undefined')
			$scope.entries[index].toggle = !$scope.entries[index].toggle;
		else
			$scope.entries[parent].suites[index].toggle = !$scope.entries[parent].suites[index].toggle;
	}

	$scope.getActiveSuite = function() {
		if(typeof $scope.active.root < 0 && typeof $scope.active.parent < 0) 
			return $scope.getSuite($scope.active.index);
		else if(typeof $scope.active.root < 0)
			return $scope.getSuite($scope.active.index, $scope.active.parent);
		else
			return $scope.getSuite($scope.active.index, $scope.active.parent, $scope.active.root);
	};

	$scope.getSuite = function(index, parent, root) {
		if(typeof root === 'undefined' && typeof parent === 'undefined') 
			return $scope.entries[index];
		else if(typeof root === 'undefined' && parent)
			return $scope.entries[parent].suites[index];
		else
			return $scope.entries[root].suites[parent].suites[index];
	}

	$scope.addNewCaseFromKeyPress = function(sibling) {
		var id = addSubEntry(sibling);
	  	setTimeout(function() {
			$("#c" + id).focus();
		}, 300);
	}

	$scope.addNewStepFromKeyPress = function(sibling, index) {
		var id = addSubEntry(sibling, index);
	  	setTimeout(function() {
			$("#st" + id).focus();
		}, 300);
	}

	$scope.getPlaceHolder = function(parent, index, tag) {
		if($scope.suite.children[parent].children[index].name.length <= 0)
			return tag;
		else
			return "";
	};

	$scope.changeSuiteOrder = function(id, newPosition) {
		var index = id.replace("suite", "");
		if(index == newPosition - 1)
			return;
		if(newPosition > index)
			newPosition--;
		var temp = $scope.entries[index];
		//console.log(index + ", " + id + ", " + newPosition);
		$scope.entries.splice(index, 1);
		if($scope.entries <= newPosition)
			newPosition = $scope.entries.length;
		$scope.entries.splice(newPosition, 0, temp);
	};

	$scope.changeCaseOrder = function(id, newPosition) {
		var index = id.replace("case", "");
		if(index == newPosition - 1)
			return;
		if(newPosition > index)
			newPosition--;
		var temp = $scope.suite.children[index];
		//console.log(index + ", " + id + ", " + newPosition);
		$scope.suite.children.splice(index, 1);
		if($scope.suite.children.length <= newPosition)
			newPosition = $scope.suite.children.length;
		$scope.suite.children.splice(newPosition, 0, temp);
	};

	$scope.changeStepOrder = function(id, newPosition, parent) {
		//console.log(index + ", " + id + ", " + newPosition + ", " + parent);
		var index = id.replace("step", "");
		if(index == newPosition - 1)
			return;
		if(newPosition > index)
			newPosition--;
		var temp = $scope.suite.children[parent].children[index];
		$scope.suite.children[parent].children.splice(index, 1);
		if($scope.suite.children[parent].children.length <= newPosition)
			newPosition = $scope.suite.children[parent].children.length;
		$scope.suite.children[parent].children.splice(newPosition, 0, temp);
	};

	$scope.reverseZindex = function(e) {
		$(e).css({'z-index': zIndex--, 'position': 'relative'});
	}

	$scope.addIf = function(add, ifNum, compNum) {
		if(ifNum == compNum)
			return compNum + add;
		else
			return compNum;
	};

	function sizeByType(type) {
		var size = "";
		switch(type) {
			case "suite" :
				size = "-lg";
				break;
			case "step" :
				size = "-sm";
				break;
			default :
				size = "";
				break;
		}
		return size;
	};

	$scope.getMetrics = function(index) {		
		var url = "http://localhost:3579/api/results" ; //This URL is from the Phoenix API's default localhost
														//Start The API .EXE file before attempting to run Requests
		var config = {
            params: {             
                format: "json",
              //  rvlimit: 50,
                file: "test1",//works,               
                callback: "JSON_CALLBACK"    
            }           
		};

        $http.jsonp(url, config).success(function(data){            
            console.log(data);
	        //alert(data.results[0].passed); 
            //console.log(result);
		    $("#myModal").find(".modal-title").html('<center>'+ $scope.suite.name + " - Metrics ! </center>");
		    $("#myModal").find(".modal-body").html('<canvas id="myChart" width="400" height="400"></canvas>'+
			    '<div class="form-group">'+ 
			    '<center><table id="table">'+ '<ul class="legend"><li><span class="superawesome"></span>Product Failure</li></ul>'+ 
			    '<ul class="legend"><li><span class="awesome"></span>Test Success</li></ul>'+ 
			    '<ul class="legend"><li><span class="notawesome"></span>Test Failure</li></ul></center>'+
			    '<table border="1" style="width:100%"><tr>'+
            '<th>Results</th>'+'<tr><td>Status: ' + data.results[0].status+ '</tr></td>'
            +'<tr><td>Test File: '+ data.file +'</tr></td>'
            +'<tr><td>Passed: '+    data.results[0].passed +'</tr></td>'
            +'<tr><td>Failed: '+    data.results[0].failed +'</tr></td>'
            +'<tr><td>Ignored: '+   data.results[0].ignored +'</tr></td>'
            +'</div></table></td>');		
		    $("#myModal").modal('show');
		
 		    var ctx = $("#myChart").get(0).getContext("2d"); 

            var data = [
                {
                    value: data.results[0].failed /*results.failed*/,
                    color:"#F7464A",
                    highlight: "#FF5A5E",
                    label: "Product Failure",
                    labelColor : 'Black',
                    labelFontSize : '16'
                },
                {
                    value: data.results[0].passed /*results.passed*/,
                    color: "#46BFBD",
                    highlight: "#5AD3D1",
                    label: "Test Success"
                },
                {
                    value: data.results[0].ignored /*results.ignored*/,
                    color: "#FDB45C",
                    highlight: "#FFC870",
                    label: "Test Failure"
                }
            ];

        var options = { 
          legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
        };
        var chart = new Chart(ctx).Doughnut(data, options);
 		})
		
		.error(function(data){
			console.log(data);
    		// called asynchronously if an error occurs
    		// or server returns response with an error status.
    	    $("#myModal").find(".modal-title").html($scope.suite.name + "<center>Test Results Not Available</center>");
		    $("#myModal").find(".modal-body").html(
			    '<div class="form-group">'+
		    	    '<center><label for="exampleInputFile">This test has not been run before. </label></center>'+		    	
		    	    '<center><p class="help-block">Tip: Automate this test before viewing metrics</p></center>'+
		  	    '</div>');
		    $("#myModal").modal('show');
  		 });
        
        //var legend = chart.generateLegend();
        //$(".modal-body").append(legend);
	}

		$scope.runAutomation = function(index) {
			var url = "http://localhost:3579/api/run" ; //This URL is from the Phoenix API's default localhost
														//Start The API .EXE file before attempting to run Requests 

			var xhr = new XMLHttpRequest();    
			xhr.open('GET', url);
			xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
    		xhr.setRequestHeader('Access-Control-Allow-Methods', 'POST');
    		xhr.setRequestHeader('Access-Control-Allow-Methods', 'DELETE');	

/*		var config = {

            params: {             
                format: "json",
              //  rvlimit: 50,
                user: "TestUser",
                team: "v-Team",
                file: $scope.suite.name, //file path of test run
                environment: "default",
                callback: "JSON_CALLBACK" 
            }   

            headers: {
            Access-Control-Allow-Origin: '*'
        			}        
        };*/

        var req = {
 
 					method: 'POST',
 					url: 'http://localhost:3579/api/run',
 					headers: {
   								'Access-Control-Allow-Origin': '*'
 							 },
 					data: { //format: "json",
              		        //  rvlimit: 50,
                	        user: "TestUser",
                	        team: "v-Team",
                	        file: $scope.suite.name, //file path of test run
                	        environment: "default",
                	        //callback: "JSON_CALLBACK"   },
							}
				}

        var reqD = {
 					method: 'DELETE',
 					url: 'http://localhost:3579/api/run',
 					headers: {
   								'Access-Control-Allow-Origin': '*'
 							 },
 					data: { //format: "json",
              		        //  rvlimit: 50,               
                	        file: $scope.suite.name, //file path of test run                
                            //callback: "JSON_CALLBACK"   },
					      }
				}

		$http(req).success(function(){});
		$http(reqD).success(function(){});        

	}

	$scope.getAttachements = function(index) {
		$("#myModal").find(".modal-title").html($scope.suite.children[index].name + " Attachement(s)");
		$("#myModal").find(".modal-body").html(
			'<div class="form-group">'+
		    	'<label for="exampleInputFile">File Upload</label>'+
		    	'<input type="file" id="exampleInputFile">'+
		    	'<p class="help-block">Upload attachments here.</p>'+
		  	'</div>');
		$("#myModal").modal('show');
	}

	/* utilities */

	Array.prototype.clone = function() {
		return this.slice(0);
	};

	Array.prototype.deepClone = function() {
		return JSON.parse(JSON.stringify(this));
	};

  });


angular.module('initProjApp').directive("tfContextmenu", function() {
	return function(scope, element, attributes) {
		$(element).bind("contextmenu", function(e) {
	    	e.preventDefault();
	    	$("<div class='custom-menu'>Custom menu</div>").appendTo("body").css({top: e.pageY + "px", left: e.pageX + "px"});
		});
		return false;
	};
});

angular.module('initProjApp').directive("tfTooltip", function() {
	return function(scope, element, attributes) {
		element.tooltip();
		return false;
	};
});

angular.module('initProjApp').directive("tfReversezorder", function() {
	return function(scope, element, attributes) {
		scope.reverseZindex(element);
		return false;
	};
});

angular.module('initProjApp').directive("tfDraggable", function() {
	return function(scope, element, attributes) {
		element.bind("dragstart", function(e) {
			var dragEle = $(element).closest(".data-drag");
			dragEle.addClass("dragging");
			dragParent = dragEle.attr("data-drag");
	    	dragId = e.target.id;
		});
		
		element.bind("dragend", function(e) {
			$(element).closest(".drag-container").removeClass("dragging");
			$(".droppable").removeClass("drag-hover");
	    	dragId = -1;
		});
		return false;
	};
});

angular.module('initProjApp').directive("tfDrop", function() {
	return function(scope, element, attributes) {
		element.bind("dragover", function(e) {
			if(dragParent == element.attr("data-drag")) {
				ignoreDrag(e);
				$(this).addClass("drag-hover");
			}
		});
		element.bind("dragleave", function(e) {
			if(dragParent == element.attr("data-drag")) {
				ignoreDrag(e);
				$(this).removeClass("drag-hover");
			}
		});
		element.bind("drop", function(e) {
			if(dragParent == element.attr("data-drag")) {
				ignoreDrag(e);
				if($("#" + dragId).hasClass("test-suite"))
					scope.changeSuiteOrder(dragId, attributes.tfDrop);
				else if($("#" + dragId).hasClass("test-case"))
					scope.changeCaseOrder(dragId, attributes.tfDrop);
				else if($("#" + dragId).hasClass("test-step"))
					scope.changeStepOrder(dragId, attributes.tfDrop, $("#" + dragId).attr("parent"));
				scope.$apply();
			}
		});
		return false;
	};
});

// this is rough, still setup with debugging.
angular.module('initProjApp').directive("tfProcesskey", function() 
{
	return function(scope, element, attributes) {
		var className = attributes.tfProcesskey;
	  	element.bind("keydown", function(e) {
	  		keys[e.which] = true; // add key to current press combo
	  		if(e.which == 13) {
	  			e.preventDefault();
	  			if($(element).attr("e-type") == "suite" && $(element).val().length > 0)
	  				scope.addNewCaseFromKeyPress(0);
	  			else if($(element).attr("e-type") == "case" && $(element).val().length > 0) {
	  				scope.addNewCaseFromKeyPress(Number($(element).attr("entry")) + 1);
	  			} else if($(element).attr("e-type") == "step" && $(element).val().length > 0) {
	  				scope.addNewStepFromKeyPress(Number($(element).attr("step-entry")) + 1, $(element).attr("entry"));
	  			}
		  		scope.$apply();
	  		}
	  		else if(keys[9] && keys[16]) {
	  			e.preventDefault();
	  			if($(element).attr("e-type") == "step") {
	  				scope.addNewCaseFromKeyPress(Number($(element).attr("entry")) + 1);
					scope.suite.children[$(element).attr("entry")].children.splice($(element).attr("step-entry"), 1);
					scope.suite.children[Number($(element).attr("entry")) + 1].name = $(element).val();
					scope.$apply();
	  			}
	  			keys = {};
	  		} else if(e.which == 9) {
	  			e.preventDefault();
	  			if($(element).attr("e-type") == "case" && $(element).attr("entry") > 0) {
	  				var title = $(element).val();
	  				scope.suite.children.splice($(element).attr("entry"), 1);
	  				scope.addNewStepFromKeyPress(scope.suite.children[$(element).attr("entry") - 1].children.length, ($(element).attr("entry") - 1));
	  				scope.toggle($(element).attr("entry") - 1, true);
	  				scope.suite.children[$(element).attr("entry") - 1].children[scope.suite.children[$(element).attr("entry") - 1].children.length - 1].name = title;
	  				scope.$apply();
	  			} else {
		  			var id = $(element).attr("id");
		  			var likeElements = $('input.tf-proc-control');
		  			var eq = likeElements.index( $("#" + id) );
		  			var ele = likeElements.eq(eq + 1);
		  			if(ele.length > 0)
		  				ele.focus();
		  			else
		  				$(".edit-title").eq(0).focus();
		  		}
		  		keys = {};
	  		}
	  	});

		element.bind("keyup", function() {
			keys = {};
		});
	    
	};
});


// drag utilities
function ignoreDrag(e) {
  e.stopPropagation();
  e.preventDefault();
}
