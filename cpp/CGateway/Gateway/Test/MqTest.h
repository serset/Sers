/*
 * main.c
 *
 *  Created on: 2019年3月13日
 *      Author: root
 */

#include "../Sers/Mq/SocketMq/ClientMq.h"

#include <string>

#include "../Sers/Core/Module/Api/ApiClient.hpp"
#include "../Sers/Core/Module/Log/Logger.hpp"
#include "../Sers/Core/Util/Common/CommonHelp.hpp"
#include "../Sers/Core/Util/ConfigurationManager/JsonFile.hpp"




//////////////////////////////////////////////////////////////////
// Logger test

using namespace Sers::Core::Util::ConfigurationManager;
using namespace Sers::Core::Module::Log;

void LoggerTest(){

	//(x.1) Logger
	Logger::Info("start");
	Logger::Error("throws exception");
	Sers::Core::Module::Log::Logger::Error("fail");


	//(x.2)ConfigurationManager
	string t3=JsonFile::ConfigurationManager.GetStringByPath("Sers.test");


	//(x.3)JsonFile
	Sers::Core::Util::ConfigurationManager::JsonFile json("appsettings.json");
	string t2=json.GetStringByPath("Sers.test");
}


// Logger test
//////////////////////////////////////////////////////////////////






//////////////////////////////////////////////////////////////////
// Mq test

using namespace std;
using namespace Sers::Core::Data;


void  OnReceiveRequest(Sers::Mq::SocketMq::MqConnect  & mq,const ArraySegment & requestData,ByteData  & replyData )
{
	replyData.InsertData(requestData);
}



Sers::Core::Util::Threading::AutoResetEvent mEvent;
void OnDisConnected()
{
	printf("mq stoped");
	mEvent.Set();
}

void MqTest(){


 	Sers::Mq::SocketMq::ClientMq mq;
 	//mq.config.host="192.168.10.100";
 	mq.config.host="127.0.0.1";
 	mq.config.port=10345;
 	mq.config.secretKey="SersSocketMq";
 	mq.OnDisConnected_Set(&OnDisConnected);

 	mq.OnReceiveRequest_Set(OnReceiveRequest);

 	if(!mq.Connect())
 	{
 		printf("连接服务器 fail.  exit");
 		return ;
 	}

    printf("mq connect succeed! ");


    mEvent.WaitOne();
	printf("main stoped");
}


// Mq test
//////////////////////////////////////////////////////////////////






















