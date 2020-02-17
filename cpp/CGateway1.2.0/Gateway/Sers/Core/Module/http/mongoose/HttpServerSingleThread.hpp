#pragma once

#include <string>
#include <string.h>
#include <unordered_map>
#include <unordered_set>
#include <functional>
#include "mongoose.h"

#include <utility>
#include "../../../Data/ByteData.hpp"
#include "../../../Module/json/Json.hpp"
#include "../../../Module/Api/ApiClient.hpp"

namespace Sers {

using namespace std;




 
typedef void FuncOnHttpRequest(HttpApiContext & httpContext);

using HttpRequestHandler = std::function<FuncOnHttpRequest>;



class HttpServerSingleThread
{
public:
	HttpServerSingleThread() {}
	~HttpServerSingleThread() {}


	static HttpRequestHandler OnHttpRequest;


	// 启动httpserver In SingleThread
	bool Start(){
			mg_mgr_init(&m_mgr, NULL);
			mg_connection *connection = mg_bind(&m_mgr, m_port.c_str(), OnHttpWebsocketEvent);
			if (connection == NULL)
				return false;
			// for both http and websocket
			mg_set_protocol_http_websocket(connection);

			printf("starting http server at port: %s\n", m_port.c_str());
			// loop
			while (true)
				mg_mgr_poll(&m_mgr, 500); // ms

			return true;
		}

	// 关闭
	bool Close(){
		mg_mgr_free(&m_mgr);
		return true;
	}
	// 端口
	std::string m_port;
private:
	// 静态事件响应函数
	static void OnHttpWebsocketEvent(mg_connection *connection, int event_type, void *event_data)
	{
		// 区分http和websocket
		if (event_type == MG_EV_HTTP_REQUEST)
		{
			http_message *http_req = (http_message *)event_data;
			HandleHttpEvent(connection, http_req);
		}
		else if (event_type == MG_EV_WEBSOCKET_HANDSHAKE_DONE ||
			     event_type == MG_EV_WEBSOCKET_FRAME ||
			     event_type == MG_EV_CLOSE)
		{
//			websocket_message *ws_message = (struct websocket_message *)event_data;
//			HandleWebsocketMessage(connection, event_type, ws_message);
		}
	}
 
	//str demo "\nheader1:123\nheader2:88"
	static  void HandleHttpEvent_HeaderForeach(Json& item,void* lp){
		string* str=(string*)lp;

		str->append("\r\n");
		str->append(item.GetName());
		str->append(":");
		str->append(item.ToString());
	}


	static void HandleHttpEvent(mg_connection *connection, http_message *http_req)
	{
		HttpApiContext context;
		
		//(x.1) init request
		//(x.x.1) route url method
		context.route = string(http_req->uri.p, http_req->uri.len);
		string query_string = string(http_req->query_string.p, http_req->query_string.len);
		context.rpc_method = string(http_req->method.p, http_req->method.len);
		//string body = string(http_req->body.p, http_req->body.len);
		if(query_string.empty()){
			context.rpc_url=context.route;		}
		else{
			context.rpc_url=context.route+"?"+query_string;
		}
		//(x.x.2) request_body
		if(http_req->body.len>0){
			context.request_body.CopyFrom(http_req->body.p, http_req->body.len);
		}else if(!query_string.empty()){
			Json query;
			query.CreateObject();
			ParseQueryString(http_req->query_string.p,http_req->query_string.len,query);
			string queryValue=query.ToString();
			context.request_body.CopyFrom(queryValue.c_str(),queryValue.length());
		}



		//(x.x.2) init request header
		context.request_header.CreateObject();
		string header_name,header_value;
		for(int t=0;t<MG_MAX_HTTP_HEADERS;t++){
			mg_str & name=http_req->header_names[t];
			if(name.len<=0) break;
			header_name.assign(name.p,name.len);

			mg_str & value=http_req->header_values[t];
			header_value.assign(value.p,value.len);

			context.request_header.SetValue(header_name,header_value);
		}


		//(x.2) callback
		OnHttpRequest(context);

		//(x.3) send reply

		//(x.3.1)send header
		//reply_header demo "\nheader1:123\nheader2:88"
		string reply_header("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked");
		context.response_header.Foreach(HandleHttpEvent_HeaderForeach,(void*) &reply_header);
		reply_header.append("\r\n\r\n");
		mg_printf(connection, reply_header.c_str());
		connection->flags |= MG_F_SEND_AND_CLOSE;

		// (x.3.2)send body
		mg_send_http_chunk(connection, context.response_body.GetData(),context.response_body.len);

		// 发送空白字符快，结束当前响应
		mg_send_http_chunk(connection, "", 0);
		//connection->flags |= MG_F_SEND_AND_CLOSE;

		//test1
//		  mg_printf(connection, "HTTP/1.0 200 OK\r\n\r\n[I am Hello1]");
//		  connection->flags |= MG_F_SEND_AND_CLOSE;


		//test2
//		mg_printf(connection, "HTTP/1.1 200 OK\r\nContent-Type:application/json; charset=UTF-8\r\nServer:Kestrel\r\nTransfer-Encoding: chunked\r\n\r\n");
//		mg_send_http_chunk(connection, "{\"a\":1}",7);//
//		// 发送空白字符快，结束当前响应
//		mg_send_http_chunk(connection, "", 0);
//		connection->flags |= MG_F_SEND_AND_CLOSE;

	}


	// buf:  a=1&b=2&c=3
	static void ParseQueryString(const char * buf,int len, Json& json) {

		const char * cur=buf;
		const char * end=buf+len;

		const char * tmp;
		char name[256],value[1000];
		while(cur<end){

			//(x.1) get name
			tmp = (char *) memchr(cur, '=', (size_t)(end - cur));
			if(!tmp) break;
			mg_url_decode(cur,tmp-cur,name,256,1);
			cur=tmp+1;

			//(x.2) get value
			tmp = (const char *) memchr(cur, '&', (size_t)(end - cur));
			if(!tmp) {
				tmp=end;
			}
			mg_url_decode(cur,tmp-cur,value,1000,1);
			cur=tmp+1;
			json.SetValue(name,value);
		}
	}


	mg_mgr m_mgr;          // 连接管理器
};





} /* namespace Sers */
