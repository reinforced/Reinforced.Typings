module Reinforced.Typings.Samples.Difficult.CodeGenerators.Pages {
    import JQueryController = Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers.JQueryController;

    export class IndexPage {
        constructor() {
            $('#btnSimpleInt').click(this.btnSimpleIntClick.bind(this));
            $('#btnMethodWithParameters').click(this.btnMethodWithParametersClick.bind(this));
            $('#btnReturningObject').click(this.btnReturningObjectClick.bind(this));
            $('#btnReturningObjectWithParameters').click(this.btnReturningObjectWithParametersClick.bind(this));
            $('#btnVoidMethodWithParameters').click(this.btnVoidMethodWithParametersClick.bind(this));
        }

        private btnSimpleIntClick() {
            JQueryController.SimpleIntMethod('#loading', '#btnSimpleInt')
                .then(r => $('#result').html('Server tells us ' + r));
        }

        private btnMethodWithParametersClick() {
            JQueryController.MethodWithParameters(Math.random(), 'string' + Math.random(), Math.random() > 0.5, '#loading', '#btnMethodWithParameters')
                .then(r => {
                $('#result').html(r);
            });
        }

        private btnReturningObjectClick() {
            JQueryController.ReturningObject('#loading', '#btnReturningObject')
                .then(r => {
                var s = "<pre> { <br/>";
                for (var key in r) {
                    s += `  ${key}: ${r[key]},\n`;
                }
                s += '} </pre>';
                $('#result').html(s);
            });
        }

        private btnReturningObjectWithParametersClick() {
            var str = 'Random number: ' + Math.random() * 100;
            JQueryController.ReturningObjectWithParameters(str, '#loading', '#btnReturningObjectWithParameters')
                .then(r => {
                var s = "<pre> { <br/>";
                for (var key in r) {
                    s += `  ${key}: ${r[key]},\n`;
                }
                s += '} </pre>';
                $('#result').html(s);
            });
        }

        private btnVoidMethodWithParametersClick() {
            JQueryController.VoidMethodWithParameters({
                Message: 'Hello',
                Success: true,
                CurrentTime: null
            }, '#loading', '#btnVoidMethodWithParameters')
                .then(() => {
                $('#result').html('Void method executed but it does not return result');
            });
        }
    }
} 