var QueryController = (function () {
    function QueryController() {
    }
    QueryController.query = function (url, data, progressSelector, disableSelector) {
        if (disableSelector === void 0) { disableSelector = ''; }
        var promise = jQuery.Deferred();
        var query = {
            data: data,
            type: 'post',
            dataType: 'json',
            timeout: 9000000,
            traditional: false,
            complete: function (response) {
                if (progressSelector && progressSelector.length > 0) {
                    $(progressSelector).remove('p[data-role]="progressContainer"');
                }
                if (disableSelector && disableSelector.length > 0) {
                    $(disableSelector).prop('disabled', false);
                }
                promise.resolve(response);
            },
            error: function (request, status, error) {
                promise.reject({ Success: false, Message: error.toString(), Data: error });
            }
        };
        if (progressSelector && progressSelector.length > 0) {
            $(progressSelector).append('<p data-role="progressContainer"> Loading ... </p>');
        }
        if (disableSelector && disableSelector.length > 0) {
            $(disableSelector).prop('disabled', true);
        }
        $.ajax(url, query);
        return promise;
    };
    return QueryController;
})();
//# sourceMappingURL=query.js.map