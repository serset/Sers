
//------------------------------------------------------------------------
var serviceStation = new sers.ServiceStation();

//apiInvokeArray
/* 
    // apiInvoke  {route: '/JsStation/api', httpMethod: 'GET', name: 'call api in js server', description: 'js作为服务站点', onInvoke,onInvokeAsync}
    //      onInvoke:   (requestData_bytes,rpcData_object,replyRpcData_object)=>{ return replyData_bytes; }	 //onInvoke 和 onInvokeAsync 指定其一即可
    //      onInvokeAsync:   (requestData_bytes,rpcData_object,replyRpcData_object,onInvokeFinish)=>{ }
    //					    onInvokeFinish :(replyData_bytes)=>{ }

    //demo:
    {
        route: '/JsStation/api', httpMethod: 'GET', name: 'call api in js server', description: 'js作为服务站点',
            onInvoke: function (requestData_bytes, rpcData_object, replyRpcData_object) {
                var request_string = vit.bytesToString(requestData_bytes);
                vit.logger.info('[api调用] request:' + request_string);

                var replyData = {
                    success: true,
                    data: {
                        request_string: request_string,
                        _: Math.random()
                    }
                };
                return vit.objectSerializeToBytes(replyData);
            }
    }
//*/
var apiInvokeArray =
    [
        {
            route: '/JsStation/api', httpMethod: 'GET', name: 'call api in js server', description: 'js作为服务站点',
            onInvoke: function (requestData_bytes, rpcData_object, replyRpcData_object) {
                var request_string = vit.bytesToString(requestData_bytes);
                vit.logger.info('[api调用] request:' + request_string);

                var replyData = {
                    success: true,
                    data:
                    {
                        request_string: request_string,
                        _: Math.random()
                    }
                };
                return vit.objectSerializeToBytes(replyData);
            }
        }
    ];





//------------------------------------------------------------------------
//(x.1)appsettings.json
var appsettings =
{
    CL: {
        host: 'ws://127.0.0.1:4503',
        secretKey: 'SersCL'
    },
    serviceStationInfo: {
        serviceStationName: 'JsStation',
        serviceStationKey: '',
        stationVersion: '',
        info: {}
    }
};

//------------------------------------------------------------------------
//(x.2)event

//(x.x.1)register logger.onmessage
//type: info/error
//e: pass error when type is error
//function(message,type,e){ }
vit.logger.onmessage = function (message, type, e) {
    if (e) message = message + '\n' + e.stack;

    console.log(message);
    txt_log.value = txt_log.value + '\n' + message;
    txt_log.scrollTop = txt_log.scrollHeight;
};

//(x.x.2)register localApiService.onError
//(Error e,requestData_bytes,rpcData_object,replyRpcData_object)
//localApiService.onError = (e,requestData_bytes,rpcData_object,replyRpcData_object)=>{ return {success:false}; }
serviceStation.localApiService.onError = function (e, requestData_bytes, rpcData_object, replyRpcData_object) {
    vit.logger.error(e);
    var reply = {
        success: false,
        error: {
            errorMessage: e.message,
            errorDetail: { name: e.name, stack: e.stack }
        }
    };
    return reply;
};

//(x.x.3)register onDisconnected from serviceCenter
serviceStation.org.event_onDisconnected = function () {
    vit.logger.info('[sers.CL]org.event_onDisconnected');
};


//------------------------------------------------------------------------
//(x.4)startService

function startService() {
    try {
        vit.logger.info('');
        vit.logger.info('--------------------------------------------');

        //(x.1)load localApi
        vit.logger.info('[ApiLoader] load localApi...');
        serviceStation.localApiService.clearApiNodes();
        serviceStation.localApiService.addApiInvokeArray(apiInvokeArray);
        vit.logger.info('loaded localApi，count：' + apiInvokeArray.length);


        //(x.2)load configuration
        vit.logger.info('load configuration...');

        //设置websocket host 地址
        serviceStation.org.setHost(appsettings.CL.host);

        //连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        serviceStation.org.secretKey = appsettings.CL.secretKey;

        serviceStation.serviceStationInfo = appsettings.serviceStationInfo;

        //(x.3)connect
        serviceStation.start();
    } catch (e) {
        vit.logger.error(e);
    }
}





//------------------------------------------------------------------------
//(x.5) call api
serviceStation.apiClient.callApiAsync("/JsStation/api1", { name: 'sers' }, 'GET',
    function (isSuccess, replyData_bytes, replyRpcData_object) {
        if (!isSuccess) {
            vit.logger.info("接口调用失败！");
            return;
        }
        //var apiRet = vit.bytesToObject(replyData_bytes); 
        var str = vit.bytesToString(replyData_bytes);
        vit.logger.info("接口调用成功。 reply:" + vit.bytesToString(replyData_bytes));
    });






//------------------------------------------------------------------------
//(x.9)stopService
function stopService() {
    try {
        vit.logger.info('');
        vit.logger.info('--------------------------------------------');
        vit.logger.info('断开连接...');
        serviceStation.stop();
        vit.logger.info('连接已断开');
    } catch (e) {
        vit.logger.error(e);
    }
}