class QueryController {
    public static query<T>(url: string, data: any, progressSelector: string, disableSelector:string = ''): JQueryPromise<T> {
        var promise = jQuery.Deferred();
        var query = {
            data: JSON.stringify(data),
            type: 'post',
            dataType: 'json',
            contentType:'application/json',
            timeout: 9000000,
            traditional: false,
            complete: () => {
                if (progressSelector && progressSelector.length > 0) {
                    $(progressSelector).find('span[data-role="progressContainer"]').remove();
                }
                if (disableSelector && disableSelector.length > 0) {
                    $(disableSelector).prop('disabled', false);
                }
            },
            success: (response: T) => {
                promise.resolve(response);
            },
            error: (request, status, error) => {
                promise.reject({ Success: false, Message: error.toString(), Data: error });
            }
        };

        if (progressSelector && progressSelector.length > 0) {
            $(progressSelector).append('<span data-role="progressContainer"> Loading ... </span>');
        }

        if (disableSelector && disableSelector.length > 0) {
            $(disableSelector).prop('disabled',true);
        }

        $.ajax(url,<any>query);
        return promise;
    }
} 