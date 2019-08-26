/*
 * ClientMq.h
 *
 *  Created on: 2019年3月18日
 *      Author: root
 */

#ifndef SERS_MQ_SOCKETMQ_CLIENTMQ_H_
#define SERS_MQ_SOCKETMQ_CLIENTMQ_H_


#include "MqConnect.h"
#include "unistd.h"



namespace Sers {




class ClientMq {

private:
	MqConnect _mqConnect;

	LongTaskHelp ping_BackThread ;

public:
	MqConnectConfig config;
	ClientMq()
	{
	}

	virtual ~ClientMq()
	{

	}

	bool IsConnected(){ return _mqConnect.IsConnected();}

	//will be called when disconnected
	void OnDisConnected_Set(void(*OnDisConnected)())
	{
		_mqConnect.OnDisConnected_Set(OnDisConnected);
	}



	//(x.1) Connect Close
	bool Connect()
	{
		//服务已启动
		if (IsConnected()) return false;

		//(x.1)连接服务器
		Logger::Info( string("[SersMq Socket Client] connect to server,host:")+config.host+" port:"+ std::to_string(config.port));

		if(!_mqConnect.Init(config))
		{
			Logger::Info("[SersMq Socket Client] connect to server fail");
			_mqConnect.Close();
			return false;
		}

		//(x.2) check secretKey

		ByteData  secretKey_requestData;
		ArraySegment secretKey;
		secretKey.CopyFrom(config.secretKey.c_str(),config.secretKey.length());
		secretKey_requestData.InsertData(secretKey);
		ArraySegment reply=SendRequest(secretKey_requestData);
		if(memcmp(reply.GetData(),"true",4)!=0)
		{
			Logger::Info("[SersMq Socket Client] check secretKey fail");
			_mqConnect.Close();
			return false;
		}



		ping_BackThread.Action_Set(&Ping_Thread,this);
		ping_BackThread.Start();

		Logger::Info("[SersMq Socket Client] connected to server");
		return true;
	}

	void Close()
	{
		ping_BackThread.Stop_Temp();
		if (!IsConnected()) return;

		Logger::Info("[SersMq Socket Client] start disconnect");
		_mqConnect.Close();
		Logger::Info("[SersMq Socket Client] disconnected to server");
	}



	//(x.2) Message

	void SendMessage(ByteData& msgData)
	{
		_mqConnect.SendMessage(msgData);
	}
	void OnReceiveMessage_Set(void(*OnReceiveMessage)(ArraySegment& msg))
	{
		_mqConnect.OnReceiveMessage=OnReceiveMessage;
	}


	//(x.3) ReqRep
	void OnReceiveRequest_Set(void (*OnReceiveRequest)( MqConnect  & mq,const ArraySegment & requestData,ByteData  & replyData ))
	{
		_mqConnect.OnReceiveRequest=OnReceiveRequest;
	}



	inline ArraySegment SendRequest(ByteData& requestData)
	{
		return _mqConnect.SendRequest(requestData);
	}

	//(x.4) Ping_Thread
   static void Ping_Thread(void*lp)
	{

	   ClientMq* mq=(ClientMq*)lp;
	   MqConnect & _mqConnect= mq->_mqConnect;
	   int pingInterval= mq->config.pingInterval/1000;


	   while (mq->IsConnected())
		{
		   if (!_mqConnect.Ping())
			{
			   _mqConnect.Close();
				return;
			}
			sleep(pingInterval);
		}
	}





};

} /* namespace Sers */

#endif /* SERS_MQ_SOCKETMQ_CLIENTMQ_H_ */
