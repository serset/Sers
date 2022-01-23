/*
 * sers.ServiceStation.js
 * Date   : 2022-01-23
 * Version: 2.1.17-temp
 * author : Lith
 * email  : serset@yeah.net
 */

; sers = { version: '2.1.17-temp' };

/*
 * vit.js 扩展
 * author : Lith
 * email  : serset@yeah.net
 */
; (function (vit) {

	// vit工具函数
	; (function () {

		vit.stringToBytes = function (str) {
			if (!str) return [];
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
		};


		vit.bytesToString = function (bytes) {
			if (!bytes) return null;
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
		};

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
		vit.arrayConcat = function (a, b, count) {
			//a.push.apply(a, b);
			if (!count) count = b.length;
			for (var t = 0; t < count; t++) {
				a.push(b[t]);
			}
			return a;
		};

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
			return (S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4());
		};

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

		//type: info/error
		//e: pass error when type is error
		//function(message,type,e){ }
		logger.onmessage;


		logger.error = function (e, message) {

			var msgBody = new Date().pattern("[mm:ss.S]") + '[error]' + (message || e.message || '');
			//console.log(e);
			//console.log(msgBody);
			try {
				if (logger.onmessage) logger.onmessage(msgBody, 'error', e);
			} catch (e) {
			}
		};

		logger.info = function (message) {
			var msgBody = new Date().pattern("[mm:ss.S]") + '[info]' + message;
			//console.log(msgBody);
			try {
				if (logger.onmessage) logger.onmessage(msgBody, 'info');
			} catch (e) {
			}
		};
	})(vit.logger = {});

})('undefined' === typeof (vit) ? vit = {} : vit);


/*
* sers.CL.js 扩展
* author : Lith
* email  : serset@yeah.net
*/
; (function (CL) {

	var logger = vit.logger;

	function PipeFrame() {

		this.write = function (arrayBuffer) {
			queueBuff.push(new Uint8Array(arrayBuffer));
			buffLen += arrayBuffer.byteLength;
		};


		//DataView list
		var queueBuff = [];
		var buffLen = 0;


		var QueueBuff_dataLenOfRemoved = 0;

		//return bytes
		function read(lenToPop) {
			if (buffLen < lenToPop) {
				return;
			}

			buffLen -= lenToPop;


			var dataToPop = [];
			var copyedIndex = 0;
			while (copyedIndex < lenToPop) {
				var leftCount = lenToPop - copyedIndex;

				var cur = queueBuff[0];
				if (QueueBuff_dataLenOfRemoved != 0) {
					cur = cur.subarray(QueueBuff_dataLenOfRemoved);
				}

				if (cur.length <= leftCount) {
					//dataToPop 数据长
					vit.arrayConcat(dataToPop, cur);

					copyedIndex += cur.length;
					QueueBuff_dataLenOfRemoved = 0;

					queueBuff.shift();
				}
				else {
					//queueBuff 数据长
					vit.arrayConcat(dataToPop, cur, leftCount);
					copyedIndex += leftCount;
					QueueBuff_dataLenOfRemoved += leftCount;
				}
			}
			return dataToPop;
		};


		var fileLen = -1;

		//return bytes
		this.readSersFile = function () {
			if (fileLen < 0) {
				var fileLen_bytes = read(4);
				if (!fileLen_bytes) {
					return null;
				}
				fileLen = vit.bytesGetInt32(fileLen_bytes, 0);
			}

			if (buffLen < fileLen) return null;

			var data = read(fileLen);
			fileLen = -1;
			return data;
		};
	};


	CL.DeliveryClient = function () {
		var self = this;
		self.host = "ws://127.0.0.1:4503";


		//function (bytes) { }
		self.event_onGetFrame;

		// function () { }
		self.event_onDisconnected;

		self.sendFrame = function (bytes) {
			if (!webSocket) throw new Error('连接尚未建立，无法发送数据,请先建立连接');
			vit.bytesInsertInt32(bytes, 0, bytes.length);
			var dataView = vit.bytesToDataView(bytes);
			webSocket.send(dataView);
		};

		var pipe = new PipeFrame();

		var webSocket = null;

		//callback: function(success){ }
		self.connect = function (callback) {
			if (webSocket) throw new Error('连接尚未断开，不可再次连接');

			webSocket = new WebSocket(self.host);
			webSocket.binaryType = "arraybuffer";

			webSocket.onerror = function (event) {
				self.close();
			};

			webSocket.onclose = function () {
				self.close();
			};

			//成功被调用 或者超时被调用
			var isCalled = false;
			var onCall = function (success) {
				if (isCalled) return;
				isCalled = true;
				callback(success);
			};
			setTimeout(onCall, 10000);

			webSocket.onopen = function (event) {
				onCall(true);
			};

			webSocket.onmessage = function (event) {
				var arrayBuffer = event.data;
				pipe.write(arrayBuffer);

				//bytes
				var frame;
				while (frame = pipe.readSersFile()) {
					try {
						self.event_onGetFrame(frame);
					} catch (e) {
						logger.error(e);
					}
				}
			};
		};


		self.close = function () {
			if (!webSocket) throw new Error('尚未建立连接，无需断开');


			//(x.1) close socket
			try {
				webSocket.close();
			} catch (e) {
				logger.error(e);
			}
			webSocket = null;

			logger.info('[sers.CL]DeliveryClient.event_onDisconnected');

			//(x.2) event_onDisconnected
			if (self.event_onDisconnected) {
				try {
					self.event_onDisconnected();
				} catch (e) {
					logger.error(e);
				}
			}

		};
	};


	function RequestAdaptor() {

		var EFrameType = { request: 1, reply: 2, message: 3 };
		var ERequestType = { app: 0, heartBeat: 1 };
		var organizeVersion = "Sers.Mq.Socket.v1";

		var self = this;

		//  requestKey -> requestCallback
		var organizeToDelivery_RequestMap = {};

		var reqKeyIndex = 100;

		//事件，向外部delivery发送字节流时被调用
		//function (bytes) { }
		self.event_onSendFrame;

		//事件，delivery向Organize发送请求时被调用
		//function (apiRequestMessage_bytes, callback) { }
		//	callback: function(apiReplyMessage_bytes){ }
		self.event_onGetRequest;

		//请求超时时间（单位ms，默认300000）
		self.requestTimeoutMs = 300000;

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
					self.event_onGetRequest(requestData, function (apiReplyMessage_bytes) {
						deliveryToOrganize_sendReply(reqKey_bytes, apiReplyMessage_bytes);
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



		//callback: ({success,replyData})=>{ }
		self.sendRequest = function (requestType,requestData, callback) {
			var reqKey = reqKeyIndex++;

			//成功被调用 或者超时被调用
			var isCalled = false;
			var onRequestFinish = function (success,replyData) {
                if (isCalled) return;
                isCalled = true;

                //if (!success)
                delete organizeToDelivery_RequestMap[reqKey];

                if (callback)
					callback({ success, replyData });
			};
			setTimeout(onRequestFinish, self.requestTimeoutMs);

			organizeToDelivery_RequestMap[reqKey] = function (replyData) { onRequestFinish(true,replyData); };

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

		// 返回对象  {reqKey:reqKey,reqKey_bytes:reqKey_bytes, oriData:oriData}
		function unpackReqRepFrame(reqRepFrame) {
			var reqKey = vit.bytesGetInt32(reqRepFrame, 0);
			return { reqKey: reqKey, reqKey_bytes: reqRepFrame.slice(0, 8), oriData: reqRepFrame.slice(8) };
		}

	}


	//websocketHost demo: "ws://127.0.0.1:4503"
	CL.OrganizeClient = function (websocketHost) {

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


		//初始化requestAdaptor 和 delivery
		(function () {

			delivery.event_onGetFrame = function (bytes) {
				requestAdaptor.deliveryToOrganize_onGetMessageFrame(bytes);
			};

			requestAdaptor.event_onGetRequest = function (apiRequestMessage_bytes, callback) {
				self.event_onGetRequest(apiRequestMessage_bytes, callback);
			};

			requestAdaptor.event_onSendFrame = function (bytes) {
				delivery.sendFrame(bytes);
			};

			delivery.event_onDisconnected = function () {
				if (self.event_onDisconnected)
					self.event_onDisconnected.apply(self, arguments);
			};

		})();
	 

		//function (event) { }
		self.event_onDisconnected = null;

		//function (apiRequestMessage_bytes,callback) { }
		//	callback function(apiReplyMessage_bytes){}
		self.event_onGetRequest = null;

		//callback: ({success,replyData})=>{ }
		self.sendRequest = function (requestData, callback) {
			requestAdaptor.sendRequest(null,requestData, callback);
		};

		//callback:   function (success) { }
		self.connect = function (callback) {

			delivery.connect(function (success) {
				//(x.1)连接不成功
				if (!success)
					callback(false);

				//(x.2)进行权限校验
				//setTimeout(function () {
				self.sendRequest(vit.stringToBytes(self.secretKey), function ({ success, replyData }) {

					//(x.x.1)请求不成功
					if (!success) {
						callback(false);
						return;
					}

					//(x.x.2)验证不成功
					if (vit.bytesToString(replyData) != 'true') {
						callback(false);
						return;
					}

					//(x.x.3)验证成功
					callback(true);
				});
				//}, 5000);
			});
		};

		self.stop = function () {
			delivery.close();
		};
	}

})(sers.CL || (sers.CL = {}));



/*
* sers.ServiceStation.js 扩展
* author : Lith
* email  : serset@yeah.net
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



		self.package = function () {
			return ApiMessage.package(rpcContextData_OriData, value_OriData);
		};


		self.unpackage = function (oriData) {

			var files = ApiMessage.unpackage(oriData);

			rpcContextData_OriData = files[0];
			value_OriData = files[1];
		};
	};

	//(bytes files)
	//return bytes
	ApiMessage.package = function () {
		var files = arguments;
		var oriData = [];

		for (var t = 0; t < files.length; t++) {
			var file = files[t];
			vit.arrayConcat(oriData, vit.int32ToBytes(file.length));
			vit.arrayConcat(oriData, file);
		}
		return oriData;
	};


	//(bytes oriData)
	//return  bytes fileArray
	ApiMessage.unpackage = function (oriData) {
		var files = [];

		var curIndex = 0;
		while (curIndex < oriData.length) {
			var fileLength = vit.bytesGetInt32(oriData, curIndex);
			var fileContent = oriData.slice(curIndex + 4, curIndex + 4 + fileLength);

			curIndex += 4 + fileLength;
			files.push(fileContent);
		}
		return files;
	};


    sers.ApiMessage = ApiMessage;

    //ApiClient
    sers.ApiClient = function (organizeClient) {

        //(string route, object arg, string httpMethod, function callback)
        //	callback: function({success,replyData_bytes,replyRpcData_object})
        this.callApiAsync = function (route, arg, httpMethod, callback) {
            var apiRequestMessage = new ApiMessage();
            apiRequestMessage.initAsApiRequestMessage(route, arg, httpMethod);

            organizeClient.sendRequest(apiRequestMessage.package(), function ({ success, replyData }) {
                if (!callback) return;
                if (!success) {
                    callback({ success: false });
                } else {
                    var apiMessage = new ApiMessage();
                    apiMessage.unpackage(replyData);

                    var value = apiMessage.getValueBytes();
                    var replyRpcData = apiMessage.getRpcData();

                    callback({ success: true, replyData_bytes: value, replyRpcData_object: replyRpcData });
                }
            });
        };

    };

	//LocalApiService
	sers.LocalApiService = function () {
		var self = this;

		// route_httpMethod -> ApiNode
		// ApiNode:   {  apiDesc,onInvoke,onInvokeAsync  }  //onInvoke 和 onInvokeAsync 指定其一即可
        //      onInvoke:   (requestData_bytes,rpcData_object,replyRpcData_object)=>{ return replyData_bytes; }	 //onInvoke 和 onInvokeAsync 指定其一即可
        //      onInvokeAsync:   (requestData_bytes,rpcData_object,replyRpcData_object,onInvokeFinish)=>{ }
        //					    onInvokeFinish :(replyData_bytes)=>{ }
		var apiNodeMap = {};

		//return [  ApiNode ];
		//ApiNode  {apiDesc:apiDesc }
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


		// ApiNode:   {  apiDesc,onInvoke,onInvokeAsync  }  //onInvoke 和 onInvokeAsync 指定其一即可
        //      onInvoke:   (requestData_bytes,rpcData_object,replyRpcData_object)=>{ return replyData_bytes; }	 //onInvoke 和 onInvokeAsync 指定其一即可
        //      onInvokeAsync:   (requestData_bytes,rpcData_object,replyRpcData_object,onInvokeFinish)=>{ }
        //					    onInvokeFinish :(replyData_bytes)=>{ }
		self.addApiNode = function (apiNode) {		 
			var apiKey = apiNode.apiDesc.route + '_' + apiNode.apiDesc.extendConfig.httpMethod;
            apiNodeMap[apiKey] = apiNode;
        };


        // apiInvoke  {route: '/JsStation/api', httpMethod: 'GET', name: 'call api in js server', description: 'js作为服务站点', onInvoke,onInvokeAsync}
        //      onInvoke:   (requestData_bytes,rpcData_object,replyRpcData_object)=>{ return replyData_bytes; }	 //onInvoke 和 onInvokeAsync 指定其一即可
        //      onInvokeAsync:   (requestData_bytes,rpcData_object,replyRpcData_object,onInvokeFinish)=>{ }
        //					    onInvokeFinish :(replyData_bytes)=>{ }
        self.addApiInvoke = function (apiInvoke) {
            var apiDesc = {
                route: apiInvoke.route,
                name: apiInvoke.name,
                description: apiInvoke.description,
                extendConfig: {
					httpMethod: apiInvoke.httpMethod
				}
			};
			self.addApiNode({ apiDesc, onInvoke: apiInvoke.onInvoke, onInvokeAsync: apiInvoke.onInvokeAsync });
		};

        // apiInvoke  {route: '/JsStation/api', httpMethod: 'GET', name: 'call api in js server', description: 'js作为服务站点', onInvoke,onInvokeAsync}
        //      onInvoke:   (requestData_bytes,rpcData_object,replyRpcData_object)=>{ return replyData_bytes; }	 //onInvoke 和 onInvokeAsync 指定其一即可
        //      onInvokeAsync:   (requestData_bytes,rpcData_object,replyRpcData_object,onInvokeFinish)=>{ }
        //					    onInvokeFinish :(replyData_bytes)=>{ }
		self.addApiInvokeArray = function (apiInvokeArray) {
			for (var apiInvoke of apiInvokeArray) {
				self.addApiInvoke(apiInvoke);
			}
		};

		//(Error e,requestData_bytes,rpcData_object,replyRpcData_object)
	    //localApiService.onError = (e,requestData_bytes,rpcData_object,replyRpcData_object)=>{ return {success:false}; }
		self.onError = function (e, requestData_bytes, rpcData_object, replyRpcData_object) {
			logger.error(e);
			var reply = {
				success: false,
				error: {
					errorMessage: e.message,
					errorDetail: { name: e.name, stack: e.stack }
				}
			};
			return reply;
		};

		//invoke local api
		//callback: (apiReplyMessage_bytes)=>{ }
		self.invokeApiAsync = (apiRequestMessage_bytes, callback)=>{

			//(x.1) 解析请求数据
			var apiMessage = new ApiMessage();
			apiMessage.unpackage(apiRequestMessage_bytes);

			var rpcData_object = apiMessage.getRpcData();
			var requestData_bytes = apiMessage.getValueBytes();

			//(x.2)解析路由获得 处理函数
			var route = rpcData_object.route;
			var httpMethod = rpcData_object.http.method;
			var apiKey = route + '_' + httpMethod;
			var apiNode = apiNodeMap[apiKey];

	

			//(x.3)进行处理获得结果数据
			var replyRpcData_object = {}, replyData_bytes;
			var onInvokeFinish = (replyData_bytes) => {
				//返回结果数据
				var apiReplyMessage_bytes = ApiMessage.package(
					vit.objectSerializeToBytes(replyRpcData_object),
					replyData_bytes
				);
				callback(apiReplyMessage_bytes);
			};

			if (apiNode) {
				
				try {
                    if (apiNode.onInvoke) {
                        replyData_bytes = apiNode.onInvoke(requestData_bytes, rpcData_object, replyRpcData_object);
                    } else if (apiNode.onInvokeAsync) {
                        apiNode.onInvokeAsync(requestData_bytes, rpcData_object, replyRpcData_object, onInvokeFinish);
                        return;
                    }
					
				} catch (e) {					
					var reply = self.onError(e, requestData_bytes, rpcData_object, replyRpcData_object);
					replyData_bytes = vit.objectSerializeToBytes(reply);
				}

			} else {
				var reply = {
					success: false,
					error: {
						errorCode: 404,
						errorMessage: "Api Not Found",
						errorDetail: { source: 'from JsStation' }
					}
				};

				replyData_bytes = vit.objectSerializeToBytes(reply);
			} 
			onInvokeFinish(replyData_bytes);
		};
	};


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
			//self.org.event_onDisconnected = function () {
			//    logger.info('[sers.CL]org.event_onDisconnected');
			//};

			self.org.event_onGetRequest = self.localApiService.invokeApiAsync;
		})();


		//(x.3) ApiClient
		(function () {
			self.apiClient = new sers.ApiClient(self.org);
		})();

		//(x.4)
		self.stop = function () {
			logger.info('[sers.ServiceStation] try stop...');
			self.org.stop();
			logger.info('[sers.ServiceStation] stoped.');
		};

		//(x.5)
		self.serviceStationInfo = {
			serviceStationName: 'JsStation', serviceStationKey: '', stationVersion: '', info: {}
		};

		//(x.6)
		var deviceInfo = { deviceKey: ('' + Math.random()).substr(2) };

		//(x.7)
		//callback: function(success){}
		self.start = function (callback) {

			logger.info('[sers.CL] try connect...');
			self.org.connect(function (success) {

				if (!success) {
					logger.info('[sers.CL] org cannot connect to server!');
					if (callback) callback(false);
					return;
				}


				//向服务中心注册localApiService
				logger.info('[ServiceStation] regist serviceStation to ServiceCenter...');

				var apiNodes = self.localApiService.getApiNodes();

				var serviceStationData = {
					serviceStationInfo: self.serviceStationInfo,
					deviceInfo: deviceInfo,
					apiNodes: apiNodes
				};

				//(string route, object arg, string httpMethod, function callback)
				//	callback: function({success,replyData_bytes,replyRpcData_object})
				self.apiClient.callApiAsync("/_sys_/serviceStation/regist", serviceStationData, 'POST', function ({ success, replyData_bytes, replyRpcData_object }) {

					if (!success) {
						logger.info("[ServiceStation] regist - failed");
						if (callback) callback(false);
						return;
					}

					var apiRet = vit.bytesToObject(replyData_bytes);

					if (!apiRet.success) {
						logger.info("[ServiceStation] regist - failed. reply:" + vit.bytesToString(replyData_bytes));
						if (callback) callback(false);
						return;
					}
					logger.info("[ServiceStation] regist - succeed");
					if (callback) callback(true);
				});
			});
		};
	}

})(sers);

