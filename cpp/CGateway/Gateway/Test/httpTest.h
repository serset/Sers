#pragma once


#include "../Sers/Core/Module/http/mongoose/HttpServer.hpp"

using namespace Sers;


void _OnHttpRequest(HttpApiContext & httpContext){
	//httpContext.response_body=httpContext.url+httpContext.request_body;
	httpContext.response_body.CopyFrom("<html>dssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss123456</html>");
}

void HttpTest(){

	HttpServer::OnHttpRequest=_OnHttpRequest;


	HttpServer http_server;
	http_server.m_port="80";

	http_server.Start();



}
