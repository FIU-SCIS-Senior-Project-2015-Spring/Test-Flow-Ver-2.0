// this file adds copy and paste ability to the test flow object
TestFlow.prototype.copyChildren = function (children) {
    _this.scope.copyBuffer = index;
}

TestFlow.prototype.copy = function (item) {
    _this.scope.copyBuffer = item;
}

TestFlow.prototype.paste = function (index) {
    if (_this.scope.copyBuffer === false)
        return;
    var len = _this.scope.suite.children[index].children.length;
    var tmp = _this.scope.suite.children[_this.scope.copyBuffer].children.deepClone();
    // alter step parents, also attempt to give uniqueness to object, else repeater will fail.
    tmp = tmp.map(function (x) {
        x.name += " >> from " + _this.scope.suite.children[_this.scope.copyBuffer].name;
        x.id = _this.scope.createId();
        x.parent = _this.scope.suite.children[index].id;
        return x;
    });
    var len = _this.scope.suite.children[index].children.length;
    if (len > 0) {
        _this.scope.suite.children[index].children = _this.scope.suite.children[index].children.concat(tmp);
    }
    else
        _this.scope.suite.children[index].children = tmp;
    if (len < _this.scope.suite.children[index].children.length) {
        _this.scope.copyBuffer = false;
        _this.notify('Copy Success!', 'The children were copied successfully.');
    } else {
        _this.notify('Copy Error', 'Copy operation was not successful, entries still in buffer.', true);
    }
}

TestFlow.prototype.hasCopyBuffer = function () {
    return _this.scope.copyBuffer !== false;
}