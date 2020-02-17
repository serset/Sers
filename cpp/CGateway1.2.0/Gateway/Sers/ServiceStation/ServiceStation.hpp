#pragma once


#include <string>

#include "../Core/Module/Log/Logger.hpp"

#include "../Mq/SocketMq/ClientMq.h"


#include "../Core/Module/Api/ApiClient.hpp"
#include "../Core/Util/Common/CommonHelp.hpp"
#include "../Core/Util/ConfigurationManager/JsonFile.hpp"

#include "../Core/Module/App/SersApplication.hpp"


namespace Sers{

	using namespace std;



	class ServiceStation{
	private:


		static ClientMq mq;




		static void Mq_OnDisConnected()
		{
			//printf("mq stoped");
			SersApplication::Stop();
		}

		static ArraySegment Mq_SendRequest(ArraySegment & request)
		{
			ByteData req;
			req.InsertData(request);
			return mq.SendRequest(req);
		}


		static void ReadConfig(MqConnectConfig & config){
			config.host= JsonFile::ConfigurationManager.GetStringByPath("Sers.Mq.Socket.host");
			config.port= JsonFile::ConfigurationManager.GetIntByPath("Sers.Mq.Socket.port");
			config.secretKey=JsonFile::ConfigurationManager.GetStringByPath("Sers.Mq.Socket.secretKey");

			config.workThreadCount=JsonFile::ConfigurationManager.GetIntByPath("Sers.Mq.Socket.workThreadCount");

			config.pingTimeout=JsonFile::ConfigurationManager.GetIntByPath("Sers.Mq.Socket.pingTimeout");
			config.pingRetryCount=JsonFile::ConfigurationManager.GetIntByPath("Sers.Mq.Socket.pingRetryCount");
			config.pingInterval=JsonFile::ConfigurationManager.GetIntByPath("Sers.Mq.Socket.pingInterval");

			config.requestTimeout=JsonFile::ConfigurationManager.GetIntByPath("Sers.Mq.Socket.requestTimeout");

			config.secretKey=JsonFile::ConfigurationManager.GetStringByPath("Sers.Mq.Socket.secretKey");
		}

	public:


		static bool Start(){

				//(x.1) ClientMq init
				ReadConfig(mq.config);
				mq.OnDisConnected_Set(Mq_OnDisConnected);

				if(!mq.Connect())
				{
					Logger::Info("[ServiceStation]connect to server failed.");
					return false;
				}

				//(x.2) Init ApiClient
				ApiClient::OnSendRequest=Mq_SendRequest;

				//(x.3) Regist ServiceStation to ServiceCenter
				string route("/_sys_/serviceStation/regist");
				string serviceStationData=JsonFile::ConfigurationManager.GetStringByPath("Sers.Gateway.StationRegist_RegistArg");
				Json  api_reply;

				ApiClient::CallApi(route,serviceStationData,api_reply);
				bool success=false;
				if(!api_reply.IsEmpty()){
					if(api_reply.GetValue("success",success)){
						if(!success){
							string error("[Regist ServiceStation to ServiceCenter]");
							error+=api_reply.ToString();
							Logger::Error(error);
						}
					}
				}

				if(success){
					Logger::Info("[Regist ServiceStation to ServiceCenter]success");


					//regist stop action to SersApplication
					auto stopMq=[&mq](){
						mq.Close();
					};
					SersApplication::AddActionOnstop(stopMq);

				}else{
					Logger::Info("[Regist ServiceStation to ServiceCenter]fail");
					mq.Close();
				}

				return success;

		}


	};

}

