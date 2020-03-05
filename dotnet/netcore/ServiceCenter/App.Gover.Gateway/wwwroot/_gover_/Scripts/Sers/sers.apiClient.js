/*
* sers.apiClient 扩展
* sers.apiClient
* Date  : 2019-02-19
* author:lith 
 * 
 */



; (function (scope) {

    var objName = 'apiClient';

    if (scope[objName]) return;
 

    var obj = {};

    scope[objName] = obj;


    var ssConfig = {
        apiHost: location.protocol+'//'+ location.hostname + ':' + location.port //   "http(s)://127.0.0.1:6000"
    };

    // sers.apiClient.ajax({ url:'http://a.com/a',type:'POST',header:{},data:{},onSuc:function(apiRet){ } });
    // obj.ajax({ api:'/a',type:'POST',header:{},data:{},onSuc:function(apiRet){ } });
    obj.ajax = function (param) {

        var url = param.url, type= param.type||'GET', header = param.header, data = param.data,  onSuc = param.onSuc;

        if (!url) {
            if (param.api) {
                url =  ssConfig.apiHost + param.api;
            }
        }

        if (data != null && data != undefined && type!='GET') {
            if (typeof (data) != 'string') {
                data = JSON.stringify(data);
            }
        }
       
        $.ajax({
            type: type,
            data: data,
            url: url,
            beforeSend: function (request) {
                if (header) {
                    for (var key in header) {
                        request.setRequestHeader(key, header[key]);
                    }
                }               
            },
            success: function (apiRet) {
                if (!apiRet.success) {
                    alert('操作失败。请重试。' + ((apiRet.error || {}).errorMessage || ''));
                    return;
                }
                onSuc(apiRet);
            }
        });
    };



    // sers.apiClient.post({ api:'/a',arg:{},onSuc:function(apiRet){ } });
    // sers.apiClient.post({ url:'http://aaa.com/a',arg:{},onSuc:function(apiRet){ } });
    obj.post = function (param) {

        var arg = param.arg, url = param.url, onSuc = param.onSuc;

        if (!url) {
            if (param.api) {
                url = ssConfig.apiHost + param.api;
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


    
    obj.apiDesc_getActive = function (arg,onSuc) {
        obj.ajax({ api: '/_gover_/apiDesc/getActive', type: 'GET', data: arg, onSuc: onSuc }); 
    };

    obj.apiDesc_getAll = function (arg,onSuc) {
        obj.ajax({ api: '/_gover_/apiDesc/getAll', type: 'GET', data: arg, onSuc: onSuc }); 
    };



    obj.serviceStation_getAll = function (onSuc) {
        obj.post({ api: '/_gover_/serviceStation/getAll', onSuc: onSuc });  
    };


    obj.serviceStation_start = function (connKey, onSuc) {
        obj.post({ api: '/_gover_/serviceStation/start', arg: { connKey: connKey}, onSuc: onSuc });  
    };

    obj.serviceStation_pause = function (connKey, onSuc) {
        obj.post({ api: '/_gover_/serviceStation/pause', arg: { connKey: connKey }, onSuc: onSuc });  
    };

    obj.serviceStation_stop = function (connKey, onSuc) {
        obj.post({ api: '/_gover_/serviceStation/stop', arg: { connKey: connKey }, onSuc: onSuc });
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
