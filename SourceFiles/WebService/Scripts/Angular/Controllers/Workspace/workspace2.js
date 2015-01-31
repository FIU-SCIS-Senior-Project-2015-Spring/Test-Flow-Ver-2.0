'use strict';

/**
 * @ngdoc function
 * @name initProjApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the initProjApp
 */
angular.module('initProjApp').controller('WorkspaceCtrl', function ($scope, storage) {

    storage.bind($scope, 'entries');
  	storage.bind($scope,'idStore');

    // test flow is already defined as global
  	testFlow = new TestFlow($scope, testManager.currentProject.Name, testManager.currentTestPlan.Id);
  	testFlow.init();

	$scope.clearStorage = function() {
	    storage.clearAll();
	    localStorage.clear();
		storage.bind($scope,'entries');
		$scope.entries = [];
		$scope.addSuite();
	}

	$scope.copySteps = function(index) {
	    testFlow.copyChildren($scope.suite.children[index]);
	}

	$scope.pasteSteps = function(index) {
	    testFlow.paste($scope.suite.children[index]);
	}

	$scope.hasCopyBuffer = function() {
	    return testFlow.hasCopyBuffer();
	}

	$scope.preventDefault = function(event) {
		event.preventDefault();
	};

	$scope.makeActive = function(index, parent, root) {
	    testFlow.SuiteHelper.makeActive(index, parent, root);
        if(!$scope.suite.attributes.new)
	        $scope.getTestCases();
	}

	$scope.isActive = function(id) {
	    return testFlow.SuiteHelper.isActive(id);
	}

	$scope.addSuite = function (name, parent, root) {
	    var name = name;
		if(typeof name === "undefined" && $("#newSuiteText").val().length <= 0)
			return;
		
		if (typeof name === "undefined" && $("#newSuiteText").val().length > 0) {
		    name = $("#newSuiteText").val();
		    $("#newSuiteText").val("");
		} else
		    name = "New Suite";

	    if(typeof parent === "undefined") {
	        testFlow.SuiteHelper.addSuite(name);
	    } else if(typeof root === "undefined") {
	        testFlow.SuiteHelper.addSuite(name, $scope.entries[parent]);
	    } else {
	        testFlow.SuiteHelper.addSuite(name, $scope.entries[root].suites[parent]);
	    }
	}

	$scope.toggle = function(index, force) {
	    testFlow.CaseHelper.toggle($scope.suite.children[index], force);
	}

	$scope.toggleDetails = function(index) {
	    testFlow.CaseHelper.toggleDetails($scope.suite.children[index]);
	}

	$scope.toggleClass = function(index) {
	    return testFlow.CaseHelper.toggleClass($scope.suite.children[index]);
	}

	$scope.toggleDetailsClass = function(index) {
	    return testFlow.CaseHelper.toggleDetailsClass($scope.suite.children[index]);
	}

	$scope.toggleButton = function(index) {
		return testFlow.CaseHelper.toggleButton($scope.suite.children[index]);
	}

	$scope.toggleDetailsButton = function(index) {
	    return testFlow.CaseHelper.toggleDetailsButton($scope.suite.children[index]);
	}

    $scope.addCaseClicked = function() {
        testFlow.CaseHelper.addCase("", $scope.suite);
    };

    $scope.addStepClicked = function(index) {
        testFlow.CaseHelper.addStep("", $scope.suite.children[index]);
    };

    $scope.removeEntryClicked = function () {
        if ($scope.active.root < 0 && $scope.active.parent < 0)
            $scope.entries.splice($scope.active.index, 1);
        else if ($scope.active.root < 0)
            $scope.entries[$scope.active.parent].suites.splice($scope.active.index, 1);
        else
            $scope.entries[$scope.active.root].suites[$scope.active.parent].suites.splice($scope.active.index, 1);
        if($scope.entries.length > 0)
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

	$scope.isSuiteToggle = function(suite) {
	    return testFlow.SuiteHelper.isSuiteToggle(suite);
	};

	$scope.toggleSuite = function(suite) {
	    testFlow.SuiteHelper.toggleSuite(suite);
	}

	$scope.addNewCaseFromKeyPress = function(sibling) {
		var newCase = testFlow.CaseHelper.addCase("", $scope.suite, sibling)
	  	setTimeout(function() {
	  	    $("#c" + newCase.id).focus();
		}, 300);
	}

	$scope.addNewStepFromKeyPress = function (sibling, index) {
	    var parent = $scope.suite.children[index];
	    var newStep = testFlow.CaseHelper.addStep("", parent, sibling);
	  	setTimeout(function() {
	  	    $("#st" + newStep.id).focus();
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
		$(e).css({'z-index': testFlow.zIndex--, 'position': 'relative'});
	}

	$scope.addIf = function(add, ifNum, compNum) {
		if(ifNum == compNum)
			return compNum + add;
		else
			return compNum;
	};

	$scope.getMetrics = function(index) {
	    testFlow.getMetrics(index);
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

	$scope.changeSuite = function (suiteId) {

	}

	$scope.sync = function () {
	    testFlow.sync();
	}

	$scope.changed = function (obj) {
	    testFlow.changed(obj);
	}

	$scope.getTestCases = function () {
	    testManager.loader("Retrieving test plan suite...", true, true);
	    $.ajax({
	        url: '/api/TestCases/' + testFlow.projectId + "/" + testFlow.testPlanId + "/" + $scope.suite.id,
	        type: 'GET',
	        dataType: 'json',
	        success: function (data) {
	            testFlow.mergeTestCases(data);
	            $scope.$apply();
	            testManager.loader("", false, false);
	        },
	        error: function (data) {

	        }
	    });
	}

	$scope.reset = function () {
	    testManager.reloadWorkspaceDelegate(testFlow.projectId, testFlow.testPlanId, "", true);
	}

	testManager.reloadWorkspaceDelegate = function (projectId, testPlanId, testPlanName, reset) {
	    $.ajax({
	        url: '/api/Suites/' + projectId + "/" + testPlanId,
	        type: 'GET',
	        dataType: 'json',
	        success: function (data) {
                // set plan name
	            $scope.testPlan = testPlanName;
	            var lastProject = localStorage.getItem("project"); // get last project and testplan
	            var lastPlan = localStorage.getItem("testplan");

                // don't do anything if they are the same.
	            if (((lastPlan && lastProject) && (typeof reset == "undefined" || !reset)) || ((projectId != lastProject && testPlanId != lastPlan) && (typeof reset == "undefined" || !reset))) {
	                localStorage.setItem("entry:" + lastProject + ":" + lastPlan, JSON.stringify($scope.entries));
	                $scope.entries = JSON.parse(localStorage.getItem("entry:" + projectId + ":" + testPlanId));
	                localStorage.setItem("testPlanName", testPlanName);
	                if ($scope.entries == null || !$.isArray($scope.entries))
	                    $scope.entries = [];
	            } else if (typeof reset != "undefined" && reset) {
	                localStorage.removeItem("entry:" + projectId + ":" + testPlanId);
	                $scope.entries = [];
	                $scope.testPlan = localStorage.getItem("testPlanName");
	                $scope.$apply();
	            }

                // merge the data into plan
	            testFlow.mergeTestPlan(data);

                // set as current
	            testFlow.projectId = projectId;
	            testFlow.testPlanId = testPlanId;
	            $scope.$apply();

                // store current
	            localStorage.setItem("testplan", testPlanId);
	            localStorage.setItem("project", projectId);

	            // make the suite active
	            if ($scope.entries.length > 0)
	                $scope.makeActive(0);
	            $scope.$apply();

                // ensure loader is gone.
	            testManager.loader("", false, false);
	        },
	        error: function () {
	            $("#myModal").find(".modal-title").html("Error Requesting Project Test Plan");
	            $("#myModal").find(".modal-body").html(
                    'An error occured when requesting the test plan from TestFlow.');
	            $("#myModal").modal('show');
	        }
	    });
	    
	}


    

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
			testFlow.dragParent = dragEle.attr("data-drag");
			testFlow.dragId = e.target.id;
		});
		
		element.bind("dragend", function(e) {
			$(element).closest(".drag-container").removeClass("dragging");
			$(".droppable").removeClass("drag-hover");
			testFlow.dragId = -1;
		});
		return false;
	};
});

angular.module('initProjApp').directive("tfDrop", function() {
	return function(scope, element, attributes) {
		element.bind("dragover", function(e) {
		    if (testFlow.dragParent == element.attr("data-drag")) {
				ignoreDrag(e);
				$(this).addClass("drag-hover");
			}
		});
		element.bind("dragleave", function(e) {
		    if (testFlow.dragParent == element.attr("data-drag")) {
				ignoreDrag(e);
				$(this).removeClass("drag-hover");
			}
		});
		element.bind("drop", function(e) {
		    if (testFlow.dragParent == element.attr("data-drag")) {
				ignoreDrag(e);
				if ($("#" + testFlow.dragId).hasClass("test-suite"))
				    scope.changeSuiteOrder(testFlow.dragId, attributes.tfDrop);
				else if ($("#" + testFlow.dragId).hasClass("test-case"))
				    scope.changeCaseOrder(testFlow.dragId, attributes.tfDrop);
				else if ($("#" + testFlow.dragId).hasClass("test-step"))
				    scope.changeStepOrder(testFlow.dragId, attributes.tfDrop, $("#" + testFlow.dragId).attr("parent"));
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
	  	    testFlow.keys[e.which] = true; // add key to current press combo
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
	  		else if (testFlow.keys[9] && testFlow.keys[16]) {
	  			e.preventDefault();
	  			if ($(element).attr("e-type") == "step") {
	  			    if (!scope.suite.children[$(element).attr("entry")].children[$(element).attr("step-entry")].attributes.new)
	  			        return;
	  				scope.addNewCaseFromKeyPress(Number($(element).attr("entry")) + 1);
					scope.suite.children[$(element).attr("entry")].children.splice($(element).attr("step-entry"), 1);
					scope.suite.children[Number($(element).attr("entry")) + 1].name = $(element).val();
					scope.$apply();
	  			}
	  			testFlow.keys = {};
	  		} else if(e.which == 9) {
	  			e.preventDefault();
	  			if($(element).attr("e-type") == "case" && $(element).attr("entry") > 0) {
	  			    var title = $(element).val();
	  			    if (!scope.suite.children[$(element).attr("entry")].attributes.new)
	  			        return;
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
	  			testFlow.keys = {};
	  		}
	  	});

		element.bind("keyup", function() {
		    testFlow.keys = {};
		});
	    
	};
});


// drag utilities
function ignoreDrag(e) {
  e.stopPropagation();
  e.preventDefault();
}
