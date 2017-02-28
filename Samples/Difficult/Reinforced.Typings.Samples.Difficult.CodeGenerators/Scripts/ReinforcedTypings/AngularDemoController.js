var Reinforced;
(function (Reinforced) {
    var Typings;
    (function (Typings) {
        var Samples;
        (function (Samples) {
            var Difficult;
            (function (Difficult) {
                var CodeGenerators;
                (function (CodeGenerators) {
                    var Pages;
                    (function (Pages) {
                        var AngularDemoController = (function () {
                            function AngularDemoController(scope, angularControllerService) {
                                this.voidMethodCalledCount = 0;
                                this._controller = angularControllerService;
                                this.simpleNum = 42;
                                this.simpleS = 'Ni hao';
                                this.simpleBoolValue = false;
                            }
                            AngularDemoController.prototype.refreshRandomValue = function () {
                                var _this = this;
                                this._controller.SimpleIntMethod().then(function (c) { return _this.RandomValue = c; });
                            };
                            AngularDemoController.prototype.parametrizedMethod = function () {
                                var _this = this;
                                this._controller.MethodWithParameters(this.simpleNum, this.simpleS, this.simpleBoolValue)
                                    .then(function (c) { return _this.simpleResult = c; });
                            };
                            AngularDemoController.prototype.retrieveComplex = function () {
                                var _this = this;
                                this._controller.ReturningObject().then(function (c) {
                                    _this.objectResult = c;
                                });
                            };
                            AngularDemoController.prototype.parametrizedMethodWithObject = function () {
                                var _this = this;
                                this._controller.ReturningObjectWithParameters(this.inputEcho).then(function (c) { return _this.echoResult = c; });
                            };
                            AngularDemoController.prototype.voidMethod = function () {
                                var _this = this;
                                this._controller.VoidMethodWithParameters({
                                    CurrentTime: null,
                                    Message: 'Hello!',
                                    Success: true
                                }).then(function (c) { return _this.voidMethodCalledCount++; });
                            };
                            return AngularDemoController;
                        }());
                        Pages.AngularDemoController = AngularDemoController;
                        app.controller('AngularDemoController', ['$scope', 'Api.AngularController', AngularDemoController]);
                    })(Pages = CodeGenerators.Pages || (CodeGenerators.Pages = {}));
                })(CodeGenerators = Difficult.CodeGenerators || (Difficult.CodeGenerators = {}));
            })(Difficult = Samples.Difficult || (Samples.Difficult = {}));
        })(Samples = Typings.Samples || (Typings.Samples = {}));
    })(Typings = Reinforced.Typings || (Reinforced.Typings = {}));
})(Reinforced || (Reinforced = {}));
//# sourceMappingURL=AngularDemoController.js.map