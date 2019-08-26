#pragma once



#include "../../Data/ByteData.hpp"

#include "../json/Json.hpp"

namespace Sers {


class RpcContextData:public Json {
public:

	RpcContextData(){
		CreateObject();
	}


	RpcContextData(const char* str){
		Parse(str);
	}

	void SetRouteAndCaller(string & route,string& callerSource){
		//(x.1) init route
		SetValue("route",route);

		//(x.2) init caller
		Json caller;
		caller.CreateObject();
		string rid=CommonHelp::NewGuid();
		caller.SetValue("rid",rid);
		caller.SetValue("source",callerSource);
		SetValue("caller",caller);
	}


	void SetHttp(string & url,string & method,Json & headers){

		Json http;
		http.CreateObject();
		http.SetValue("url",url);
		http.SetValue("method",method);
		http.SetValue("headers",headers);

		SetValue("http",http);
	}


	void GetHttpHeaders(Json & headers){
		Json http;
		GetValue("http",http);

		if(http.IsEmpty()){
			headers.Clear();
			return;
		}

		string strHeaders;
		http.GetValue("headers",strHeaders);
		headers.Parse(strHeaders.c_str());
	}

};


/*
 *  rpc Demo:
 * {
        "route": "/DemoStation/v1/api/5/rpc/2",
        "caller": {
            "rid": "8320becee0d945e9ab93de6fdac7627a",
            "callStack": ["xxxx","xxxxxx"],    // parentRequestGuid array
            "source": "Outside"
        },
        "http": {
            "url": "http://127.0.0.1:6000/DemoStation/v1/api/5/rpc/2",
            "method":"GET",
            "headers": {
                "Cache-Control": "no-cache",
                "Connection": "keep-alive",
                "Content-Type": "application/javascript",
                "Accept": "* / *",
                "Accept-Encoding": "gzip, deflate",
                "Authorization": "bearer",
                "Host": "127.0.0.1:6000",
                "User-Agent": "PostmanRuntime/7.6.0",
                "Postman-Token": "78c5a1cb-764f-4e04-b2ae-514924a40d5a"
            }
        },
		"error":{
			//SsError∏Ò Ω
		}
        "user": {}
    }
 *
 *
 * */


} /* namespace Sers */
