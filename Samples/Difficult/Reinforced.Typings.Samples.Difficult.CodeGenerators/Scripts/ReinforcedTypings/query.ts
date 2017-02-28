/**
 * Simple intermediate class for jQuery to easily perform AJAX requests with disabling elements
 */
class QueryController {
    /**
     * Main query method that we will use to perform AJAX calls from jQuery middleware controller
     * @param url URL to query
     * @param data Data to send
     * @param progressSelector Element selector to add progress indicator to
     * @param disableSelector Selector of element that is needed to be disabled while request
     */
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
                    $(progressSelector).find('i[data-role="progressContainer"]').remove();
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
            $(progressSelector).append('<i data-role="progressContainer"> <span class="glyphicon glyphicon-cd spin"></span> Loading ... </i>');
        }

        if (disableSelector && disableSelector.length > 0) {
            $(disableSelector).prop('disabled',true);
        }

        $.ajax(url,<any>query);
        return promise;
    }
} 