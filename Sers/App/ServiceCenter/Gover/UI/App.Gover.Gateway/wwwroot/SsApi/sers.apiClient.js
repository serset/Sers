/*
* sers.apiClient 扩展
* sers.apiClient
* Date  : 2019-02-19
* author:lith 

    <script type="text/javascript" src="lith.import.js"></script>

    <script type="text/javascript" >
         sers.apiClient.get({
                widgetName: 'infoGet.widget'
                , depends: [
                    { type: 'js', src: '/xxx/xx.js' },
                    { type: 'css', src: '/xxx/xx.css' },
                    { type: 'css', content: '.classA{}' }
                ]
                , files: [
                    { type: 'js', src: '/xxx/xx.js' }
                ]
            });
    
    </script>
 */



; (function (scope) {

    var objName = 'apiClient';

    if (scope[objName]) return;
 

    var obj = {};

    scope[objName] = obj;


    var ssConfig = {
        apiHost: location.hostname + ':' + location.port //"127.0.0.1:6000"
    };

    // sers.apiClient.post({ api:'/a',arg:{},onSuc:function(apiRet){ } });
    // sers.apiClient.post({ url:'http://aaa.com/a',arg:{},onSuc:function(apiRet){ } });
    obj.post = function (param) {

        var arg = param.arg, url = param.url, onSuc = param.onSuc;

        if (!url) {
            if (param.api) {
                url = 'http://' + ssConfig.apiHost + param.api;
            }
        }

        var data = arg==null?null:JSON.stringify(arg);
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


    
    obj.apiDesc_getActive = function (onSuc) {
        obj.post({ api: '/_gover_/apiDesc/getActive', onSuc: onSuc });   
    };

    obj.apiDesc_getAll = function (onSuc) {
        obj.post({ api: '/_gover_/apiDesc/getAll', onSuc: onSuc });  
    };



    obj.serviceStation_getAll = function (onSuc) {
        obj.post({ api: '/_gover_/serviceStation/getAll', onSuc: onSuc });  
    };


    obj.serviceStation_start = function (connGuid, onSuc) {
        obj.post({ api: '/_gover_/serviceStation/start', arg: { connGuid: connGuid}, onSuc: onSuc });  
    };

    obj.serviceStation_pause = function (connGuid, onSuc) {
        obj.post({ api: '/_gover_/serviceStation/pause', arg: { connGuid: connGuid }, onSuc: onSuc });  
    };




    obj.apiStation_getAll = function (onSuc) {
        obj.post({ api: '/_gover_/apiStation/getAll',  onSuc: onSuc }); 
    };

    obj.apiStation_start = function (stationName, onSuc) {
        obj.post({ api: '/_gover_/apiStation/start', arg: { stationName: stationName }, onSuc: onSuc });  
    };

    obj.apiStation_pause = function (stationName, onSuc) {
        obj.post({ api: '/_gover_/apiStation/pause', arg: { stationName: stationName }, onSuc: onSuc });  
    };



})('undefined' != typeof (sers) ? sers : (sers = {}));
