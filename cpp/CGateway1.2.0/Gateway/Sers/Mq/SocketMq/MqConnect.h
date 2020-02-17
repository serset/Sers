/*
 * MqConn.h
 *
 *  Created on: 2019年3月14日
 *      Author: lith
 */

#ifndef SERS_MQ_SOCKETMQ_MQCONNECT_H_
#define SERS_MQ_SOCKETMQ_MQCONNECT_H_



#include "SocketConnect.h"

#include "../../Core/Util/Common/CommonHelp.hpp"
#include "../../Core/Util/Concurrent/ConcurrentMap.h"
#include "../../Core/Util/Threading/AutoResetEvent.h"



#define MqVersion  "Sers.Mq.Socket.v1"

namespace Sers{

using namespace Sers::Core::Util::Concurrent;


enum EMsgType{
	EMsgType_request=1,
	EMsgType_reply=2,
	EMsgType_message=3
};


enum EReqType{
	EReqType_app=0,
	EReqType_ping=1
};



struct MqConnectConfig
{
	//(x.1)workThread
	/// <summary>
	/// 后台处理消息的线程个数（单位个，默认16）
	/// </summary>
	int workThreadCount = 16;


	//(x.2)ping
	/// <summary>
	/// 心跳测试超时时间（单位ms，默认2000）
	/// </summary>
	int pingTimeout = 5000;
	/// <summary>
	/// 心跳测试失败重试次数（单位次，默认3）
	/// </summary>
	int pingRetryCount = 3;
	/// <summary>
	/// 心跳测试时间间隔（单位ms，默认1000）
	/// </summary>
	int pingInterval = 1000;

	//(x.3)request
	/// <summary>
	/// 请求超时时间（单位ms，默认300000）
	/// </summary>
	int requestTimeout = 300000;

	/// <summary>
	/// 服务端 host地址
	/// </summary>
	string host;
	/// <summary>
	/// 服务端 监听端口号
	/// </summary>
	int port = 5555;

	string secretKey;
};




struct RequestInfo
{
   ArraySegment replyData;

    AutoResetEvent * mEvent;
};


class MqConnect
{
private:
	SocketConnect socketConnect;

public:


	MqConnectConfig config;
	MqConnect()
	{
 		socketConnect.OnGetMsg_Set(&Socket_OnGetMsg,this);
    }

	~MqConnect()
	{
 		Close();
	}

	bool IsConnected()
	{
		return socketConnect.IsConnected();
	}

	//will be called when disconnected
	void OnDisConnected_Set(void(*OnDisConnected)())
	{
		socketConnect.OnDisConnected_Set(OnDisConnected);
	}


	//(x.1)  Init  Close

	/*
	 * 初始化并开启接收发送等线程
	 **/
	bool Init(MqConnectConfig config)
	{
		this->config = config;

		if(!socketConnect.Init(config.workThreadCount,config.host.c_str(),config.port))
				return false;

		if (!Ping())
		{
			socketConnect.Close();
			return false;
		}

		return true;
	}

	void Close()
	{
		if (IsConnected())
		{
			socketConnect.Close();
		}
	}





 private:

	//(x.2) socket 底层接口

	static void Socket_OnGetMsg(SocketConnect * mq,const ArraySegment& bytes,void* arg)
	{
		MqConnect * self=(MqConnect *)arg;
		char * data=bytes.GetData();

		if(bytes.len<2) return;


		switch(data[0]){
			case EMsgType_request:
			{
				EReqType requestType=(EReqType)data[1];
				long reqKey=*(long*)(data+2);

				ArraySegment  requestData=bytes.Slice(10);
				ByteData replyData;

				self->RequestManage_OnReceiveRequest(requestType,requestData,replyData);

				if(!replyData.IsEmpty())
				{
					//(x.1) reqKeyFrame
					ArraySegment reqKeyFrame;
					reqKeyFrame.CopyFrom((char*)(&reqKey),8);
				    replyData.InsertData(reqKeyFrame);
				    //(x.2) send data
					self->Socket_SendData(EMsgType_reply,requestType,replyData);
				}

				break;
			}
			case EMsgType_reply:
			{
				//char requestType=data[1];
				long reqKey=*(long*)(data+2);

				ArraySegment replyData=bytes.Slice(10);
				self->RequestManage_OnGetMsg(reqKey,replyData);
				break;
			}
			case EMsgType_message:
			{
				ArraySegment msg=bytes.Slice(2);
				self->OnReceiveMessage(msg);
				break;
			}
		}
	}



	bool Socket_SendData(EMsgType msgType, EReqType requestType, ByteData& data)
	{
		ArraySegment arr;
		arr.Malloc(2);
		char* farmeTag= arr.GetData();

		farmeTag[0] = msgType;
		farmeTag[1] = requestType;

		data.InsertData(arr);

		return socketConnect.SendMsg(data);
	}



public:

// (x.3) Message
void SendMessage(ByteData& msgData)
{
	EReqType requestType = (EReqType)0;
    Socket_SendData(EMsgType_message, requestType,msgData);
}
// callback for Message. if msgData was used by outer,msg shoud be disposed manual.
void(*OnReceiveMessage)(ArraySegment& msg);




//(x.4) RequestManage
private:
		ConcurrentMap<long,RequestInfo*> requestManage_requestMap;

		void RequestManage_OnReceiveRequest(char requestType, ArraySegment & requestData,ByteData & replyData )
			{
				switch(requestType)
				{
				case EReqType_app:
					{
						OnReceiveRequest(*this,requestData,replyData);
						return;
					}
				case EReqType_ping:
					{
							Ping_InsertMqVersion(replyData);
							return;
					}
				}
			}


	    /*
	     * return value(replyData) need to be disposed by caller. replyData can be null.
	     * prevents caller from  disposing requestData.
	     **/
		ArraySegment RequestManage_SendRequest(ByteData & requestData,EReqType requestType, int msTimeOut)
		{
			//(x.1) init request data
			long reqKey=   CommonHelp::NewGuidLong();

			ArraySegment arr;
			arr.CopyFrom((char*)&reqKey,8);
			requestData.InsertData(arr);


			//(x.2) init Request callback
			AutoResetEvent  mEvent;
			RequestInfo requestInfo;
			requestInfo.mEvent = &mEvent;
			requestManage_requestMap[reqKey]= &requestInfo;

			ArraySegment replyData;


			//(x.3) mq send request
			if(!Socket_SendData(EMsgType_request,requestType,requestData))
			{
				return replyData;
			}

			//(x.4) wait reply
			if(mEvent.WaitOne(msTimeOut))
			{
				replyData=requestInfo.replyData;
			}
			requestManage_requestMap.erase(reqKey);

			return replyData;
		}

		 void RequestManage_OnGetMsg(long reqKey, ArraySegment& replyData)
		{
			ConcurrentMap<long , struct RequestInfo *>::iterator item=requestManage_requestMap.find(reqKey);
			if(!item._isEnd)
			{
				struct RequestInfo *requestInfo=item.second;
				if(requestInfo)
				{
					requestInfo->replyData=replyData;
					requestInfo->mEvent->Set();
					return;
				}
			}

		}



	//(x.5) RepReq
 public:

	void (*OnReceiveRequest)(MqConnect  & mq,const ArraySegment & requestData,ByteData  & replyData );

	inline ArraySegment SendRequest(ByteData& requestData)
	{
		return RequestManage_SendRequest(requestData,EReqType_app, config.requestTimeout);
	}




	//(x.6) Ping
public:
	bool Ping()
	{
		int retry = config.pingRetryCount;
		while (!Ping_Try())
		{
			if ((--retry) <= 0)
			{
				return false;
			}
		}
		return true;
	}

private:

	bool Ping_Try()
	{
		ByteData requestData;
		Ping_InsertMqVersion(requestData);

		ArraySegment replyData=RequestManage_SendRequest(requestData,EReqType_ping, config.pingTimeout);


		bool success=false;
		if(replyData.len==strlen(MqVersion))
		{
			success= 0==strncmp(replyData.GetData(),MqVersion,replyData.len);
		}
	    return success;
	}

	static void Ping_InsertMqVersion(ByteData & data )
	{
		ArraySegment version;
		version.CopyFrom(MqVersion,strlen(MqVersion));
		data.InsertData(version);
	}




};













//}}
}




#endif /* SERS_MQ_SOCKETMQ_MQCONNECT_H_ */
