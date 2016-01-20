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
                        var JQueryController = Reinforced.Typings.Samples.Difficult.CodeGenerators.Controllers.JQueryController;
                        var IndexPage = (function () {
                            function IndexPage() {
                                $('#btnSimpleInt').click(this.btnSimpleIntClick.bind(this));
                                $('#btnMethodWithParameters').click(this.btnMethodWithParametersClick.bind(this));
                                $('#btnReturningObject').click(this.btnReturningObjectClick.bind(this));
                            }
                            IndexPage.prototype.btnSimpleIntClick = function () {
                                JQueryController.SimpleIntMethod('#loading', '#btnSimpleInt')
                                    .then(function (r) { return alert('Server tells us ' + r); });
                            };
                            IndexPage.prototype.btnMethodWithParametersClick = function () {
                                JQueryController.MethodWithParameters(Math.random(), 'string' + Math.random(), Math.random() > 0.5, '#loading', '#btnMethodWithParameters')
                                    .then(function (r) {
                                    $('#result').html(r);
                                });
                            };
                            IndexPage.prototype.btnReturningObjectClick = function () {
                                JQueryController.ReturningObject('#loading', '#btnReturningObject')
                                    .then(function (r) {
                                    alert((r.Success ? 'Success: ' : 'Fail: ') + r.Message + '\nTime: ' + r.CurrentTime);
                                });
                            };
                            return IndexPage;
                        })();
                        Pages.IndexPage = IndexPage;
                    })(Pages = CodeGenerators.Pages || (CodeGenerators.Pages = {}));
                })(CodeGenerators = Difficult.CodeGenerators || (Difficult.CodeGenerators = {}));
            })(Difficult = Samples.Difficult || (Samples.Difficult = {}));
        })(Samples = Typings.Samples || (Typings.Samples = {}));
    })(Typings = Reinforced.Typings || (Reinforced.Typings = {}));
})(Reinforced || (Reinforced = {}));
//# sourceMappingURL=IndexPage.js.map