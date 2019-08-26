#pragma once


#include "../Core/Module/http/mongoose/HttpServer.hpp"
#include "../Core/Module/Api/ApiClient.hpp"
#include "../Core/Module/Api/RpcData.hpp"
#include "../Core/Util/ConfigurationManager/JsonFile.hpp"

namespace Sers {


	class GatewayHelp{
	private:
		static string Rpc_CallerSource;


		static void _OnHttpRequest(HttpApiContext & httpContext){

			//(x.1)init httpContext
			httpContext.rpc_callerSource=Rpc_CallerSource;


			//(x.2) call api
			ApiClient::CallApi(httpContext);
		}

	public:


		static bool Start(){

				Rpc_CallerSource=JsonFile::ConfigurationManager.GetStringByPath("Sers.Gateway.Rpc.CallerSource");

				HttpServer::OnHttpRequest=_OnHttpRequest;


				HttpServer::m_port=JsonFile::ConfigurationManager.GetStringByPath("Sers.Gateway.WebHost.http_port");

				HttpServer::responseDefaultContentType=JsonFile::ConfigurationManager.GetStringByPath("Sers.Gateway.WebHost.ResponseDefaultContentType");

				HttpServer::responseExtHeaders=JsonFile::ConfigurationManager.GetStringByPath("Sers.Gateway.WebHost.ResponseExtHeaders");


				HttpServer::workThreadCount=Sers::JsonFile::ConfigurationManager.GetIntByPath("Sers.Gateway.WebHost.workThreadCount");
				if(HttpServer::workThreadCount<=0) HttpServer::workThreadCount=16;

				return HttpServer::Start();
		}


	};



} /* namespace Sers */
