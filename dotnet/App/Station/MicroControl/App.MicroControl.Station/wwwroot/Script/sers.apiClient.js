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
     

    // sers.apiClient.post({ api:'/a',arg:{},onSuc:function(apiRet){ } });
    // sers.apiClient.post({ url:'http://aaa.com/a',arg:{},onSuc:function(apiRet){ } });
    obj.post =function (param) {

        var arg = param.arg, url = param.url, onSuc = param.onSuc;

        if (!url) {
            if (param.api) {
                url = 'http://' + ssConfig.apiHost + param.api;
            }
        }

        var data = arg == null ? null : JSON.stringify(arg);
        $.ajax({
            type: "POST",
            data: data,
            url: url,
            //beforeSend: function (request) {
            //    request.setRequestHeader("Authorization", "Bearer " + getAt());
            //},
            success: function (apiRet) {
                if (!apiRet.success) {
                    alert('操作失败。请重试。' + ((apiRet.error || {}).errorMessage || ''));
                    return;
                }
                onSuc(apiRet);
            }
        });
    };

 



})('undefined' != typeof (sers) ? sers : (sers = {}));
