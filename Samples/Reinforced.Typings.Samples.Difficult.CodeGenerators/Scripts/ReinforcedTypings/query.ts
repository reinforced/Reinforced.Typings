class QueryController {
    public static query<T>(url: string, data: any, progressSelector: string, disableSelector:string = ''): JQueryPromise<T> {
        var promise = jQuery.Deferred();
        var query = {
            data: data,
            type: 'post',
            dataType: 'json',
            timeout: 9000000,
            traditional: false,
            complete: (response: T) => {
                if (progressSelector && progressSelector.length > 0) {
                    $(progressSelector).remove('p[data-role]="progressContainer"');
                }
                if (disableSelector && disableSelector.length > 0) {
                    $(disableSelector).prop('disabled', false);
                }
                promise.resolve(response);
            },
            error: (request, status, error) => {
                promise.reject({ Success: false, Message: error.toString(), Data: error });
            }
        };

        if (progressSelector && progressSelector.length > 0) {
            $(progressSelector).append('<p data-role="progressContainer"> Loading ... </p>');
        }

        if (disableSelector && disableSelector.length > 0) {
            $(disableSelector).prop('disabled',true);
        }

        $.ajax(url,<any>query);
        return promise;
    }
} 