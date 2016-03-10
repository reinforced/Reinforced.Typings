module Reinforced.Typings.Samples.Difficult.CodeGenerators.Pages {
    import AngularController = Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers.AngularController;

    export class AngularDemoController {
        private _controller: AngularController;
        private _scope: angular.IScope;

        public RandomValue: number;
        
        // model for simple method
        public simpleNum: number;
        public simpleS: string;
        public simpleBoolValue: boolean;
        public simpleResult: string;

        // end of model for simple method

        public objectResult: {};

        public inputEcho:string;
        public echoResult: {};

        public voidMethodCalledCount:number = 0;

        constructor(scope: angular.IScope, angularControllerService: AngularController) {
            this._controller = angularControllerService;
            this.simpleNum = 42;
            this.simpleS = 'Ni hao';
            this.simpleBoolValue = false;
        }

        public refreshRandomValue() {
            this._controller.SimpleIntMethod().then(c => this.RandomValue = c);
        }

        public parametrizedMethod() {
            this._controller.MethodWithParameters(this.simpleNum, this.simpleS, this.simpleBoolValue)
                .then(c => this.simpleResult = c);
        }

        public retrieveComplex() {
            this._controller.ReturningObject().then(c => {
                this.objectResult = c;
            });
        }

        public parametrizedMethodWithObject() {
            this._controller.ReturningObjectWithParameters(this.inputEcho).then(c => this.echoResult = c);
        }

        public voidMethod() {
            this._controller.VoidMethodWithParameters({
                CurrentTime: null,
                Message: 'Hello!',
                Success: true
            }).then(c => this.voidMethodCalledCount++);
        }

    }

    app.controller('AngularDemoController', ['$scope', 'Api.AngularController', AngularDemoController]);
} 