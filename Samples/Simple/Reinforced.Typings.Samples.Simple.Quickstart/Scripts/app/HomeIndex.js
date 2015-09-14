///<reference path="../typings/jquery/jquery.d.ts"/>
var Reinforced;
(function (Reinforced) {
    var Typings;
    (function (Typings) {
        var Samples;
        (function (Samples) {
            var Simple;
            (function (Simple) {
                var Quickstart;
                (function (Quickstart) {
                    var HomeIndexPage = (function () {
                        function HomeIndexPage() {
                            $(document).ready(this.documentReady.bind(this));
                        }
                        HomeIndexPage.prototype.documentReady = function () {
                            $('#btnRequest').click(this.onRequest.bind(this));
                            $('#btnSend').click(this.onSend.bind(this));
                        };
                        HomeIndexPage.prototype.onRequest = function () {
                            $.ajax({
                                url: '/Home/GetOrder?orderId=10',
                                success: this.handleResponse
                            });
                        };
                        HomeIndexPage.prototype.handleResponse = function (data) {
                            var text = data.ClientName + ", " + data.Address + " (" + data.ItemName + ", " + data.Quantity + ")";
                            $('#divInfo').text(text);
                        };
                        HomeIndexPage.prototype.onSend = function () {
                        };
                        return HomeIndexPage;
                    })();
                    Quickstart.HomeIndexPage = HomeIndexPage;
                })(Quickstart = Simple.Quickstart || (Simple.Quickstart = {}));
            })(Simple = Samples.Simple || (Samples.Simple = {}));
        })(Samples = Typings.Samples || (Typings.Samples = {}));
    })(Typings = Reinforced.Typings || (Reinforced.Typings = {}));
})(Reinforced || (Reinforced = {}));
window['homeIndexPage'] = new Reinforced.Typings.Samples.Simple.Quickstart.HomeIndexPage();
//# sourceMappingURL=HomeIndex.js.map