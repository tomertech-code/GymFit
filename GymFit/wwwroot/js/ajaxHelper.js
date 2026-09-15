const ajaxHelper = {
    _token: function () { return $('input[name="__RequestVerificationToken"]').first().val(); },
    _withToken: function (data) {
        const token = this._token();
        if (!token) return data;
        if (data instanceof FormData) { if (!data.has('__RequestVerificationToken')) data.append('__RequestVerificationToken', token); return data; }
        data = data || {};
        if (typeof data === 'string') return data + (data ? '&' : '') + '__RequestVerificationToken=' + encodeURIComponent(token);
        return Object.assign({}, data, { __RequestVerificationToken: token });
    },
    get: function (url, successCallback, errorCallback) { $.ajax({url:url,type:'GET',dataType:'json',success:successCallback,error:function(xhr,status,error){console.error('AJAX GET Error:',error);if(errorCallback)errorCallback(error);else showToast('error','An error occurred while fetching data');}}); },
    post: function (url, data, successCallback, errorCallback) { $.ajax({url:url,type:'POST',data:this._withToken(data),dataType:'json',success:successCallback,error:function(xhr,status,error){console.error('AJAX POST Error:',error);if(errorCallback)errorCallback(error);else showToast('error',xhr.responseJSON?.message||'An error occurred while processing your request');}}); },
    put: function (url, data, successCallback, errorCallback) { $.ajax({url:url,type:'PUT',data:this._withToken(data),dataType:'json',success:successCallback,error:function(xhr,status,error){console.error('AJAX PUT Error:',error);if(errorCallback)errorCallback(error);else showToast('error','An error occurred while updating');}}); },
    delete: function (url, successCallback, errorCallback) { $.ajax({url:url,type:'DELETE',data:this._withToken({}),dataType:'json',success:successCallback,error:function(xhr,status,error){console.error('AJAX DELETE Error:',error);if(errorCallback)errorCallback(error);else showToast('error','An error occurred while deleting');}}); }
};


window.escapeHtml = window.escapeHtml || function (value) {
    return String(value ?? '').replace(/[&<>"']/g, function (char) {
        return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[char];
    });
};
