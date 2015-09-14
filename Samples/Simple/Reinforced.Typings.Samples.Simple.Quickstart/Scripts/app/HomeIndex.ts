///<reference path="../typings/jquery/jquery.d.ts"/>

module Reinforced.Typings.Samples.Simple.Quickstart {
    import OrderViewModel = Reinforced.Typings.Samples.Simple.Quickstart.Models.IOrderViewModel;

    export class HomeIndexPage{
        constructor() {
            $(document).ready(this.documentReady.bind(this));
        }

        private documentReady() {
            $('#btnRequest').click(this.onRequest.bind(this));
            $('#btnSend').click(this.onSend.bind(this));
        }

        private onRequest() {
            $.ajax({
                url: '/Home/GetOrder?orderId=10',
                success:this.handleResponse
            });
        }

        private handleResponse(data: OrderViewModel) {
            var text = `${data.ClientName}, ${data.Address} (${data.ItemName}, ${data.Quantity})`;
            $('#divInfo').text(text);
        }

        private onSend() {

        }
    }
}

window['homeIndexPage'] = new Reinforced.Typings.Samples.Simple.Quickstart.HomeIndexPage();