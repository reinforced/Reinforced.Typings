module Reinforced.Typings.Samples.Difficult.CodeGenerators.Pages {
    import JQueryController = Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers.JQueryController;

    export class IndexPage {
        constructor() {
            $('#btnSimpleInt').click(this.btnSimpleIntClick.bind(this));
            $('#btnMethodWithParameters').click(this.btnMethodWithParametersClick.bind(this));
            $('#btnReturningObject').click(this.btnReturningObjectClick.bind(this));
        }

        private btnSimpleIntClick() {
            JQueryController.SimpleIntMethod('#loading','#btnSimpleInt')
                .then(r => alert('Server tells us ' + r));
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
                    alert((r.Success ? 'Success: ' : 'Fail: ') + r.Message + '\nTime: ' + r.CurrentTime);
                });
        }
    }
} 