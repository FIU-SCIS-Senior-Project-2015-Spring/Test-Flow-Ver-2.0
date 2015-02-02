var TestFlow = function(scope) {
	this.scope = scope;
	this.idStore = 0;
	this.elements = [];
	this.copyBuffer = "";
	this.active = 0;
	this.suite = -1;

	createSuite = function(name) {
		var newSuite = this.getAtomic();
		newSuite.name = name;
		newSuite.attributes.parent = 0;
		newSuite.attributes.size = Attributes.sizes[0];
		newSuite.attributes.order = this.elments.length;
		newSuite.bind(AtomicOperations);
		newSuite.addChildren();
		newSuite.addSuites();
		newSuite.createId();
		this.elements.push(newSuite);
	}

	createCase = function(name, parent, index) {
		var newSuite = this.getAtomic();
		newSuite.name = name;
		newSuite.id = AtomicOperations.createId();
		newSuite.attributes.parent = parent;
		newSuite.attributes.size = Attributes.sizes[1];
		newSuite.attributes.order = this.elments.length;
		newSuite.bind(AtomicOperations);
		newSuite.addChildren();
		this.elements[index].children.push(newSuite);
	}

	createSharedStep = function(name) {
		var newSuite = this.getAtomic();
		newSuite.name = name;
		newSuite.id = AtomicOperations.createId();
		newSuite.attributes.parent = 0;
		newSuite.attributes.size = Attributes.sizes[2];
		newSuite.attributes.order = this.elments.length;
		newSuite.bind(AtomicOperations);
		this.elements.push(newSuite);
	}

	createStep = function(name) {
		var newSuite = this.getAtomic();
		newSuite.name = name;
		newSuite.id = AtomicOperations.createId();
		newSuite.attributes.parent = 0;
		newSuite.attributes.size = Attributes.sizes[3];
		newSuite.attributes.order = this.elments.length;
		newSuite.bind(AtomicOperations);
		this.elements.push(newSuite);
	}

	Attributes = {
		"types" : ["suite", "test case", "shared step", "step"],
		"sizes" : ["-lg", "", "-sm", "-sm"];
	};

	getAtomic =  function() { 
		return {
			"contents" : {
				"id" : "",
				"name" : ""
			},
			"type" : null,
			"attributes" : {
				"parent" : -1,
				"size" : "",
				"toggle" : false,
				"order" : 0
			}
		};
	};

	AtomicOperations = {
		"createId" : function() {
			this.id =  this.iDStore++;
		},
		"addChildren" : function() {
			this.prototype.children = [];
		},
		"addSuites" : function(atom) {
			this.prototype.suites = [];
		}
	}
};