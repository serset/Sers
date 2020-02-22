# SsApiDesc


```javascript
//Demo:

{
   "name":"获取用户信息",
   "description":"用户必须登录",

   //路由
   "route":"/ApiStation1/path1/path2/api1.html"

 //扩展配置（json）
 ,"extendConfig":{
    //请求方式，若不指定则默认支持所有方式（demo: POST、GET、DELETE、PUT等）
    "httpMethod":"GET",

    //原始路由
    "oriRoute":"/apidemo/Values/arg1/{id}/{id2}",


    //系统生成的接口文字描述
    "sysDesc":"method:GET"
 },
 


   //请求参数类型   SsModel类型
   "argType":  { SsModel },

   //返回数据类型   SsModel类型
   "returnType":  { SsModel },


   //sers1版本使用（为了兼容，暂不禁用）
   "rpcValidations":[
		//SsLimit format:  {"path":"path in RpcContext","ssError":{ssError} , "ssValid": {SsValid} }

		{  "path":"callerSource","ssError":{} , "ssValid": {"type":"Equal","value":"ServiceStation"}   }


		/*
		{  "path":"callerSource","ssError":{} , "ssValid": {"type":"Equal","value":"ServiceStation"}   }

		{"path":"user.userType", "ssValid": [
			{"type":"Equal","value":"Logined", "errorMessage": "必须为登陆用户"}
		] } ,

		{"path":"http.method", "ssValid": [
			{"type":"Equal","value":"POST", "errorMessage": "接口只接受Post请求"}
		] }  */
   ],

   //api调用限制(rpc)，sers2版本使用
   "rpcVerify2":{
	//SsExp format:  {"type":"Switch", "body":[  {"condition":SsExp,"value":SsExp } ,...   ]   }   
   },

 
 
   ////调用api时构建RpcContext 的配置参数
   //"rpcContextBuildConfig":  { 	
   
   //},

}
```
 