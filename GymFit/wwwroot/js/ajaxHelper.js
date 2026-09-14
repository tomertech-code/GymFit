const ajaxHelper = {
    get: function (url, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                if (successCallback) successCallback(data);
            },
            error: function (xhr, status, error) {
                console.error('AJAX GET Error:', error);
                if (errorCallback) {
                    errorCallback(error);
                } else {
                    showToast('error', 'An error occurred while fetching data');
                }
            }
        });
    },

    post: function (url, data, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            dataType: 'json',
            success: function (response) {
                if (successCallback) successCallback(response);
            },
            error: function (xhr, status, error) {
                console.error('AJAX POST Error:', error);
                if (errorCallback) {
                    errorCallback(error);
                } else {
                    showToast('error', 'An error occurred while processing your request');
                }
            }
        });
    },

    put: function (url, data, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'PUT',
            data: data,
            dataType: 'json',
            success: function (response) {
                if (successCallback) successCallback(response);
            },
            error: function (xhr, status, error) {
                console.error('AJAX PUT Error:', error);
                if (errorCallback) {
                    errorCallback(error);
                } else {
                    showToast('error', 'An error occurred while updating');
                }
            }
        });
    },

    delete: function (url, successCallback, errorCallback) {
        $.ajax({
            url: url,
            type: 'DELETE',
            dataType: 'json',
            success: function (response) {
                if (successCallback) successCallback(response);
            },
            error: function (xhr, status, error) {
                console.error('AJAX DELETE Error:', error);
                if (errorCallback) {
                    errorCallback(error);
                } else {
                    showToast('error', 'An error occurred while deleting');
                }
            }
        });
    }
};
