/*
* sers.apiClient 扩展
* sers.apiClient
* Date  : 2019-02-19
* author:lith 
 

   
 */



; (function (scope) {

    var objName = 'apiClient';

    if (scope[objName]) return;
 

    var obj = {};

    scope[objName] = obj;

    var ssConfig = {
        apiHost: location.hostname + ':' + location.port //"127.0.0.1:6000"
        //apiHost: 'lj.sersms.com:6000'
    };
     


    function httpPost(param) {

        var arg = param.arg, url = param.url, onSuc = param.onSuc;

        if (!url) {
            if (param.api) {
                url = 'http://' + ssConfig.apiHost + param.api;
            }
        }

        var data = JSON.stringify(arg);
        $.ajax({
            type: "POST",
            data: data,
            url: url,
            beforeSend: function (request) {
                //request.setRequestHeader("Authorization", "Bearer " + getAt());
            },
            success: function (result) {
                onSuc(result);
            }
        });
    }   

    obj.post = httpPost;

 



})('undefined' != typeof (sers) ? sers : (sers = {}));
