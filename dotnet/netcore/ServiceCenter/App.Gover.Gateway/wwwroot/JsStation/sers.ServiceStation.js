/*
 * sers.servicestation.js 扩展  
 * Date   : 2019-12-27
 * Version: 1.0
 * author : Lith
 * email  : sersms@163.com
 */

var sers = { version:'1.0'};

/*
 * vit.js 扩展  
 * Date   : 2019-12-27
 * Version: 1.0
 * author : Lith
 * email  : sersms@163.com
 */
; (function (vit) {

    // vit工具函数
    ; (function () {

        vit.stringToBytes=function (str) {
            var bytes = new Array();
            var len, c;
            len = str.length;
            for (var i = 0; i < len; i++) {
                c = str.charCodeAt(i);
                if (c >= 0x010000 && c <= 0x10FFFF) {
                    bytes.push(((c >> 18) & 0x07) | 0xF0);
                    bytes.push(((c >> 12) & 0x3F) | 0x80);
                    bytes.push(((c >> 6) & 0x3F) | 0x80);
                    bytes.push((c & 0x3F) | 0x80);
                } else if (c >= 0x000800 && c <= 0x00FFFF) {
                    bytes.push(((c >> 12) & 0x0F) | 0xE0);
                    bytes.push(((c >> 6) & 0x3F) | 0x80);
                    bytes.push((c & 0x3F) | 0x80);
                } else if (c >= 0x000080 && c <= 0x0007FF) {
                    bytes.push(((c >> 6) & 0x1F) | 0xC0);
                    bytes.push((c & 0x3F) | 0x80);
                } else {
                    bytes.push(c & 0xFF);
                }
            }
            return bytes;


        }


        vit.bytesToString=function (bytes) {
            if (typeof bytes === 'string') {
                return bytes;
            }
            var str = '',
                _arr = bytes;
            for (var i = 0; i < _arr.length; i++) {
                var one = _arr[i].toString(2),
                    v = one.match(/^1+?(?=0)/);
                if (v && one.length == 8) {
                    var bytesLength = v[0].length;
                    var store = _arr[i].toString(2).slice(7 - bytesLength);
                    for (var st = 1; st < bytesLength; st++) {
                        store += _arr[st + i].toString(2).slice(2);
                    }
                    str += String.fromCharCode(parseInt(store, 2));
                    i += bytesLength - 1;
                } else {
                    str += String.fromCharCode(_arr[i]);
                }
            }
            return str;
        }

        vit.bytesToObject = function (bytes) {
            return eval('(' + vit.bytesToString(bytes) + ')');
        };       


        vit.objectSerializeToString = function (obj) {
            if (obj == null || typeof (obj) == 'undefined') {
                return null;
            }
            var str = obj;
            if (typeof (str) != 'string') {
                str = JSON.stringify(str);
            }
            return str;
        };


        vit.objectSerializeToBytes = function (obj) {            
            var str = vit.objectSerializeToString(obj);            
            return vit.stringToBytes(str);
        };


        //合并连个数组
        vit.arrayConcat = function (a, b) {
            a.push.apply(a, b);
            return a;
        }

        vit.bytesToArrayBuffer = function (bytes) {
            return new Uint8Array(bytes).buffer;
        };

        vit.bytesToDataView = function (bytes) {
            return new DataView(vit.bytesToArrayBuffer(bytes));
        };

        vit.arrayBufferToBytes = function (arrayBuffer) {
            return Array.from(new Uint8Array(arrayBuffer));
        };

        vit.dataViewToBytes = function (dataView) {
            return vit.arrayBufferToBytes(dataView.buffer);
        };


        vit.bytesGetInt32 = function (bytes, index) {
            return new DataView(new Uint8Array(bytes).buffer).getInt32(index || 0, true);
        };

        vit.int32ToBytes = function (int32) {
            var buffer = new ArrayBuffer(4);
            var view = new DataView(buffer);
            view.setInt32(0, int32, true);
            return vit.dataViewToBytes(view);
        };


        vit.bytesInsertInt32 = function (bytes, index, int32) {

            var bytesInt32 = vit.int32ToBytes(int32);

            bytes.splice(index, 0, bytesInt32[0], bytesInt32[1], bytesInt32[2], bytesInt32[3]);
            return bytes;
        };


        // return '112233445566778899aabbccddee'
        vit.guid = function guid() {
            function S4() {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            }
            return (S4() + S4() +  S4() + S4() +     S4() + S4() + S4() + S4());
        }  

    })();

 
    //vit.logger
    (function (logger) {
         
            /*** 对Date的扩展，将 Date 转化为指定格式的String * 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q)
            *     可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
            * eg: 
            * (newDate()).pattern("yyyy-MM-dd hh:mm:ss.S")==> 2006-07-02 08:09:04.423      
            * (new Date()).pattern("yyyy-MM-dd E HH:mm:ss") ==> 2009-03-10 二 20:09:04      
            * (new Date()).pattern("yyyy-MM-dd EE hh:mm:ss") ==> 2009-03-10 周二 08:09:04      
            * (new Date()).pattern("yyyy-MM-dd EEE hh:mm:ss") ==> 2009-03-10 星期二 08:09:04      
            * (new Date()).pattern("yyyy-M-d h:m:s.S") ==> 2006-7-2 8:9:4.18      
            */
        Date.prototype.pattern = function (fmt) {
            var o = {
                "M+": this.getMonth() + 1, //月份         
                "d+": this.getDate(), //日         
                "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
                "H+": this.getHours(), //小时         
                "m+": this.getMinutes(), //分         
                "s+": this.getSeconds(), //秒         
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度         
                "S": this.getMilliseconds() //毫秒         
            };
            var week = {
                "0": "/u65e5",
                "1": "/u4e00",
                "2": "/u4e8c",
                "3": "/u4e09",
                "4": "/u56db",
                "5": "/u4e94",
                "6": "/u516d"
            };
            if (/(y+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            if (/(E+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(fmt)) {
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
                }
            }
            return fmt;
        }; 
                   


        //function(message){}
        logger.onmessage;


        logger.error = function (e, message) {
            console.log(e);
            var msgOut = new Date().pattern("[mm:ss.S]") + '[error]' + message;
            console.log(msgOut);
            try {
                if (logger.onmessage) logger.onmessage(msgOut);
            } catch (e) {
            }
        };

        logger.info = function (message) {
            var msgBody = new Date().pattern("[mm:ss.S]") + '[info]' + message;
            console.log(msgBody);
            try {
                if (logger.onmessage) logger.onmessage(msgBody);
            } catch (e) {
            }
        };
    })(vit.logger = {});

})('undefined' === typeof (vit) ? vit = {} : vit);




/*
* sers.CL.js 扩展
* Date   : 2019-12-27
* Version: 1.0
* author : Lith
* email  : sersms@163.com
*/
; (function (CL) {

    var logger = vit.logger;

    function PipeFrame() {

        this.write = function (bytes) {
            vit.arrayConcat(receive, bytes);
        };


        //bytes
        var receive = [];

        //return bytes
        this.read = function () {
            if (receive.length < 4) {
                return;
            }

            var length = vit.bytesGetInt32(receive, 0);
            if (receive.length < length + 4) {
                return;
            }
            var bytes = receive.slice(4, length + 4);
            receive = receive.slice(length + 4);
            return bytes;
        };
    }


    CL.DeliveryClient=function () {
        var self = this;
        self.host = "ws://127.0.0.1:4503";


        //function (bytes) { }
        self.event_onGetFrame;

        // function () { }
        self.event_onDisconnected;


        self.sendFrame = function (bytes) {

            vit.bytesInsertInt32(bytes, 0, bytes.length);

            var dataView = vit.bytesToDataView(bytes);

            webSocket.send(dataView);
        };



        var pipe = new PipeFrame();

        var webSocket = null;

        //callback: function(isConnected){ }
        self.connect = function (callback) {

            webSocket = new WebSocket(self.host);
            webSocket.binaryType = "arraybuffer";

            webSocket.onerror = function (ev) {
                self.close();
            };

            webSocket.onclose = function () {
                self.close();
            };



            //成功被调用 或者超时被调用
            var isCalled = false;
            var onCall = function (isSuccess) {
                if (isCalled) return;
                isCalled = true;
                callback(isSuccess);
            };
            setTimeout(onCall, 10000);

            webSocket.onopen = function (event) {
                onCall(true);
            };


            webSocket.onmessage = function (event) {
                var arrayBuffer = event.data;
                var bytes = vit.arrayBufferToBytes(arrayBuffer);
                pipe.write(bytes);

                //bytes
                var frame;
                while (frame = pipe.read()) {
                    try {
                        self.event_onGetFrame(frame);
                    } catch (e) {
                        logger.error(e);
                    }
                }
            };

        };

        self.close = function () {
            if (!webSocket) return;

            //(x.1) close socket
            try {
                webSocket.close();
                webSocket = null;
            } catch (e) {
                logger.error(e);
            }

            //(x.2) onDisconnected
            if (self.onDisconnected) {
                try {
                    self.onDisconnected();
                } catch (e) {
                    logger.error(e);
                }
            }

        };
    }


    function RequestAdaptor() {


        var EFrameType = { request: 1, reply: 2, message: 3 };

        var ERequestType = { app: 0, heartBeat: 1 };

        const organizeVersion = "Sers.Mq.Socket.v1";


        var self = this;


        //  requestKey -> requestCallback
        var organizeToDelivery_RequestMap = {};

        var reqKeyIndex = 100;


        //事件，向外部delivery发送字节流时被调用
        //function (bytes) { }
        self.event_onSendFrame;


        //事件，delivery向Organize发送请求时被调用
        //function (requestData, callback) { }
        //callback: function(replyData){ }
        self.event_onGetRequest;


        //外部调用，当外部从delivery读取到数据时调用
        self.deliveryToOrganize_onGetMessageFrame = function (bytes) {
            //deliveryToOrganize_ProcessFrame

            var msgType = bytes[0];


            var msgData = bytes.slice(2);
            switch (msgType) {

                case EFrameType.reply:

                    var t = unpackReqRepFrame(msgData);
                    var reqKey = t.reqKey;
                    var replyData = t.oriData;

                    var reqCallback = organizeToDelivery_RequestMap[reqKey];
                    if (reqCallback) {
                        delete organizeToDelivery_RequestMap[reqKey];
                        reqCallback(replyData);
                    }
                    break;

                case EFrameType.request:

                    var t = unpackReqRepFrame(msgData);
                    var reqKey_bytes = t.reqKey_bytes;
                    var requestData = t.oriData;

                    var requestType = bytes[1];
                    deliveryToOrganize_onGetRequest(requestType, reqKey_bytes, requestData);
                    break;

                case EFrameType.message:
                    //TODO
                    break;

            }


        };

        function deliveryToOrganize_onGetRequest(requestType, reqKey_bytes, requestData) {

            switch (requestType) {
                case ERequestType.app:
                    self.event_onGetRequest(requestData, function (replyData) {
                        deliveryToOrganize_sendReply(reqKey_bytes, replyData);
                    });
                    return;
                case ERequestType.heartBeat:
                    var version = vit.bytesToString(requestData);
                    if (version == organizeVersion) {
                        // send reply
                        deliveryToOrganize_sendReply(reqKey_bytes, requestData);
                    }
                    else {
                        // send reply
                        deliveryToOrganize_sendReply(reqKey_bytes, [0]);
                    }
                    return;
            }

        }

        function deliveryToOrganize_sendReply(reqKey_bytes, replyData) {

            var repFrame = packageReqRepFrame(reqKey_bytes, replyData);
            delivery_sendFrame(EFrameType.reply, 0, repFrame);
        }



        //callback: function(replyData,isSuccess){ }
        self.sendRequest = function (requestData, callback, requestType) {
            var reqKey = reqKeyIndex++;

            //成功被调用 或者超时被调用
            var isCalled = false;
            var onCall = function (replyData, isSuccess) {
                if (isCalled) return;
                isCalled = true;

                if (!isSuccess) {
                    delete organizeToDelivery_RequestMap[reqKey];
                }
                if (callback)
                    callback(replyData, isSuccess);
            };
            setTimeout(onCall, 10000);

            organizeToDelivery_RequestMap[reqKey] = function (replyData) { onCall(replyData, true); };

            var reqKey_bytes = vit.int32ToBytes(reqKey);
            reqKey_bytes.push(0, 0, 0, 0);

            var reqRepFrame = packageReqRepFrame(reqKey_bytes, requestData);
            delivery_sendFrame(EFrameType.request, requestType || ERequestType.app, reqRepFrame);

        };



        function delivery_sendFrame(msgType, requestType, bytes) {

            bytes.splice(0, 0, msgType, requestType);

            self.event_onSendFrame(bytes);
        }

        //reqKey_bytes 8字节
        //返回 reqRepFrame[bytes]
        function packageReqRepFrame(reqKey_bytes, oriData) {
            return vit.arrayConcat(reqKey_bytes, oriData);
        }

        // 返回对象  {reqKey:reqKey, oriData:oriData}
        function unpackReqRepFrame(reqRepFrame) {

            var reqKey = vit.bytesGetInt32(reqRepFrame, 0);

            return { reqKey: reqKey, reqKey_bytes: reqRepFrame.slice(0, 8), oriData: reqRepFrame.slice(8) };
        }


    }


    //websocketHost demo: "ws://127.0.0.1:4503"
    CL.OrganizeClient=function (websocketHost) {

        var self = this;

        var delivery = new CL.DeliveryClient();


        //连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        self.secretKey = "SersCL";

        //设置websocket host 地址 demo: "ws://127.0.0.1:4503"
        self.setHost = function (websocketHost) {
            delivery.host = websocketHost;
        };

        self.setHost(websocketHost);


        var requestAdaptor = new RequestAdaptor();


        //<<<<<<<<<<<<<<<< 初始化requestAdaptor 和 delivery
        (function () {

            delivery.event_onGetFrame = function (bytes) {
                requestAdaptor.deliveryToOrganize_onGetMessageFrame(bytes);
            };


            requestAdaptor.event_onGetRequest = function (requestData, callback) {
                self.event_onGetRequest(requestData, callback);
            };


            requestAdaptor.event_onSendFrame = function (bytes) {
                delivery.sendFrame(bytes);
            };



            delivery.event_OnDisconnected = function () {
                self.event_OnDisconnected.apply(self, arguments);
            };

        })();
        //>>>>>>>>>>>>>>>>>



        //function (event) { }
        self.event_onDisconnected = null;

        //function (requestData,callback) { }
        //callback function(replyData){}
        self.event_onGetRequest = null;

        //callback: function(replyData,isSuccess){ }
        self.sendRequest = function (requestData, callback) {
            requestAdaptor.sendRequest(requestData, callback);
        }



        //callback:   function (isSuccess) { }
        self.connect = function (callback) {

            delivery.connect(function (isSuccess) {
                //(x.1)连接不成功
                if (!isSuccess)
                    callback(false);

                //(x.2)进行权限校验
                self.sendRequest(vit.stringToBytes(self.secretKey), function (replyData, isSuccess) {

                    //(x.x.1)请求不成功
                    if (!isSuccess) callback(false);

                    //(x.x.2)验证不成功
                    if (vit.bytesToString(replyData) != 'true') {
                        callback(false);
                    }

                    //(x.x.3)验证成功
                    callback(true);

                });
                return;


                setTimeout(function () {

                    self.sendRequest(vit.stringToBytes(self.secretKey), function (replyData, isSuccess) {

                        //(x.x.1)请求不成功
                        if (!isSuccess) callback(false);

                        //(x.x.2)验证不成功
                        if (vit.bytesToString(replyData) != 'true') {
                            callback(false);
                        }

                        //(x.x.3)验证成功
                        callback(true);

                    });
                }, 5000);

                return;
            });
        };

        self.stop = function () {
            delivery.close();
        };
    }
 
})(sers.CL || (sers.CL = {}));



/*
* sers.ServiceStation.js 扩展
* Date   : 2019-12-27
* Version: 1.0
* author : Lith
* email  : sersms@163.com
*/
; (function (sers) {

    var logger = vit.logger;

    //ApiMessage
    function ApiMessage() {
        var self = this;

        //bytes
        var rpcContextData_OriData;

        //bytes
        var value_OriData;

        //return object
        self.getRpcData = function () {
            var strRpc = vit.bytesToString(rpcContextData_OriData);
            if (!strRpc) return {};
            return eval('(' + strRpc + ')');
        };

        //return bytes
        self.getValueBytes = function () {
            return value_OriData;
        };

        //return string
        self.getValueString = function () {
            var strValue = vit.bytesToString(value_OriData);
            return strValue;
        };

        //return object
        self.getValueObject = function () {
            return eval('(' + self.getValueString() + ')');
        };


        self.initAsApiRequestMessage = function (route, arg, httpMethod) {
            var rpcData = {
                "route": route,
                "caller": {
                    "rid": vit.guid(),
                    "callStack": [],    // parentRequestGuid array
                    "source": "Internal"
                },
                "http": {
                    "method": httpMethod || "GET"
                }
            };
            rpcContextData_OriData = vit.objectSerializeToBytes(rpcData);
            value_OriData = vit.objectSerializeToBytes(arg);
        };


        //(bytes rpcContextData_OriData, bytes value_OriData)
        //return bytes
        ApiMessage.package = function (rpcContextData_OriData, value_OriData) {

            var oriData = vit.int32ToBytes(rpcContextData_OriData.length)
                .concat(rpcContextData_OriData,
                    vit.int32ToBytes(value_OriData.length),
                    value_OriData
                );

            return oriData;
        };


        self.package = function () {
            return ApiMessage.package(rpcContextData_OriData, value_OriData);
        };


        self.unpackage = function (oriData) {

            var files = [];

            var curIndex = 0;
            while (curIndex < oriData.length) {
                var fileLength = vit.bytesGetInt32(oriData, curIndex);
                var fileContent = oriData.slice(curIndex + 4, curIndex + 4 + fileLength);

                curIndex += 4 + fileLength;
                files.push(fileContent);
            }

            rpcContextData_OriData = files[0];
            value_OriData = files[1];
        };
    }

    //ApiClient
    sers.ApiClient = function (organizeClient) {

        //(string route, object arg, string httpMethod, function callback)
        //callback: function(isSuccess,replyData_bytes,replyRpcData_object)
        this.callApi = function (route, arg, httpMethod, callback) {
            var apiRequestMessage = new ApiMessage();
            apiRequestMessage.initAsApiRequestMessage(route, arg, httpMethod);

            organizeClient.sendRequest(apiRequestMessage.package(), function (replyData, isSuccess) {
                if (!callback) return;
                if (!isSuccess) {
                    callback(false);
                } else {
                    var apiMessage = new ApiMessage();
                    apiMessage.unpackage(replyData);

                    var rpcData = apiMessage.getRpcData();
                    var value = apiMessage.getValueBytes();

                    callback(true, value, rpcData);
                }
            });
        };
    };

    //LocalApiService
    sers.LocalApiService = function () {
        var self = this;

        // key   route_httpMehtod
        // value   {  apiDesc:apiDesc,Invoke:onInvoke  }
        // onInvoke:   function(requestData_bytes,rpcData,reply_rpcData){}
        var apiNodeMap = {};

        //return [  apiNode,  ];
        //apiNode  {apiDesc:apiDesc }
        self.getApiNodes = function () {
            var apiNodes = [];
            for (var key in apiNodeMap) {
                apiNodes.push({ apiDesc: apiNodeMap[key].apiDesc });
            }
            return apiNodes;
        };

        //清空已加载的apiNode
        self.clearApiNodes = function () {
            apiNodeMap = {};
        };

        //Invoke:   function(requestData_bytes,rpcData_object,reply_rpcData_object){}
        self.addApiNode = function (apiDesc, Invoke) {
            var apiKey = apiDesc.route + '_' + apiDesc.extendConfig.httpMethod;
            apiNodeMap[apiKey] = { apiDesc: apiDesc, Invoke: Invoke };
        }

        //(string route, string httpMethod, string description, Invoke Invoke)
        //Invoke:   function(requestData_bytes,rpcData_object,reply_rpcData_object){}
        self.addSimpleApiNode = function (route, httpMethod, description, Invoke) {
            var apiDesc = {
                route: route,
                name: description,
                description: description,
                extendConfig: {
                    httpMethod: httpMethod
                }
            };
            self.addApiNode(apiDesc, Invoke);
        }

        //apiRequestMessage bytes
        //return apiReplyMessage bytes
        self.callApi = function (apiRequestMessage) {

            //(x.1) 解析请求数据
            var apiMessage = new ApiMessage();
            apiMessage.unpackage(apiRequestMessage);

            var rpcData = apiMessage.getRpcData();
            var requestData_bytes = apiMessage.getValueBytes();

            //(x.2)解析路由获得 处理函数
            var route = rpcData.route;
            var httpMethod = rpcData.http.method;
            var apiKey = route + '_' + httpMethod;
            var apiNode = apiNodeMap[apiKey];

            //(x.3)进行处理获得结果数据
            var replyRpcDta = {}, replyData;
            if (apiNode && apiNode.Invoke) {
                try {
                    replyData = apiNode.Invoke(requestData_bytes, rpcData, replyRpcDta);
                } catch (e) {
                    logger.error(e);
                    var reply = {
                        "success": false,
                        "error": {
                            "errorMessage": e.message,
                            "errorDetail": { source: 'from JsStation' }
                        }
                    };

                    replyData = vit.objectSerializeToBytes(reply);
                }

            } else {
                var reply = {
                    "success": false,
                    "error": {
                        "errorCode": 404,
                        "errorMessage": "接口不存在",
                        "errorDetail": { source: 'from JsStation' }
                    }
                };

                replyData = vit.objectSerializeToBytes(reply);
            }

            //(x.4)返回结果数据
            var apiReplyMessage_bytes = ApiMessage.package(
                vit.objectSerializeToBytes(replyRpcDta),
                replyData
            );
            return apiReplyMessage_bytes;
        };
    }


    //ServiceStation
    sers.ServiceStation = function () {
        var self = this;


        //(x.1) LocalApiService
        (function () {
            self.localApiService = new sers.LocalApiService();
        })();

        //(x.2) OrganizeClient self.org
        (function () {

            self.org = new sers.CL.OrganizeClient("ws://127.0.0.1:4503");
            self.org.event_onDisconnected = function () {
                logger.info('[sers.CL]org.event_onDisconnected');
            };

            self.org.event_onGetRequest = function (requestData, callback) {
                var reply_bytes = self.localApiService.callApi(requestData);
                callback(reply_bytes);
            };
        })();


        //(x.3) ApiClient
        (function () {
            self.apiClient = new sers.ApiClient(self.org);
        })();

        self.stop = function () {
            logger.info('[sers.ServiceStation]try stop...');
            self.org.stop();
            logger.info('[sers.ServiceStation] stoped.');
        }


        //callback: function(isSuccess){}
        self.start = function (callback) {

            logger.info('[sers.CL]try connect...');
            self.org.connect(function (isSuccess) {

                if (!isSuccess) {
                    logger.info('[sers.CL]org cannot connect to server!');
                    if (callback) callback(false);
                    return;
                }


                //向服务中心注册localApiService
                logger.info('[ServiceStation] regist serviceStation to ServiceCenter...');
                var serviceStationInfo = {
                    serviceStationName: '', serviceStationKey: '', stationVersion: '', info: {}
                };
                var deviceInfo = { deviceKey: 'JsStation' };
                var apiNodes = self.localApiService.getApiNodes();

                var serviceStationData =
                {
                    serviceStationInfo: serviceStationInfo,
                    deviceInfo: deviceInfo,
                    apiNodes: apiNodes
                };

                //(string route, object arg, string httpMethod, function callback)
                //callback: function(isSuccess,replyData_bytes,replyRpcData_object)
                self.apiClient.callApi("/_sys_/serviceStation/regist", serviceStationData, 'POST', function (isSuccess, replyData_bytes, replyRpcData_object) {

                    if (!isSuccess) {
                        logger.info("[ServiceStation] regist - failed");
                        if (callback) callback(false);
                        return;
                    }

                    var apiRet = vit.bytesToObject(replyData_bytes);

                    if (!apiRet.success) {
                        logger.info("[ServiceStation] regist - failed. reply:" + vit.bytesToString(replyData));
                        if (callback) callback(false);
                    }
                    logger.info("[ServiceStation] regist - succeed");
                    if (callback) callback(true);
                });
            });
        };
    }

})(sers);

 