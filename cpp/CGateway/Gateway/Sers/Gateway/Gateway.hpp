#pragma once

#define SERS_CGATEWAY_VERSION "1.8.4"

#include "GatewayHelp.hpp"


#include "../ServiceStation/ServiceStation.hpp"




using namespace Sers;



void Gateway_Start()
{

	Logger::Info(string("[CGateway]version:")+ SERS_CGATEWAY_VERSION );

	if(Sers::JsonFile::ConfigurationManager.GetStringByPath("Sers.Gateway.Console_PrintLog")=="true"){
		Sers::Logger::OnLog=Sers::Logger::OnLog_Printf;
	}


	//(x.1) start ServiceStation
	if(!ServiceStation::Start()){
		Logger::Info("connect to ServiceCenter failed.program will stop.");
		SersApplication::Stop();
		return;
	}


	//(x.2) init http server service
	if(!GatewayHelp::Start()){
		Logger::Info("start http service failed.program will stop.");
		SersApplication::Stop();
		return;
	}


	//(x.3) RunAwait
	SersApplication::RunAwait();

	//(x.4) quit
	Logger::Info("program will stop.");
}

