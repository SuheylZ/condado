function sendRequest(url, requestType, dataType, onSuccess, onError) {
    $.ajax({
        url: url,
        dataType: dataType,
        type: requestType,
        success: onSuccess,
        error: onError
    });
}

var galDialerAPI = {
    getCounts: function (onSuccess, onError) {
        sendRequest('/Services/GALDialer.ashx?method=GetCounts', 'POST', 'json', onSuccess, onError)
    },
    dialAlead: function (onSuccess, onError) {
        sendRequest('/Services/GALDialer.ashx?method=DialLead', 'POST', 'json', onSuccess, onError)
    },
    getUserData: function (onSuccess, onError) {
        sendRequest('/Services/GALDialer.ashx?method=GetUserData', 'POST', 'json', onSuccess, onError)
    }
};