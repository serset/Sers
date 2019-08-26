



#ifndef SERS_MQ_SOCKETMQ_SOCKETCONNECT_H_
#define SERS_MQ_SOCKETMQ_SOCKETCONNECT_H_



#include <stdio.h>
#include <stdlib.h>



#include <sys/select.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>

#include <unistd.h>
#include <semaphore.h>
#include <memory.h>

#include "../../Core/Data/ByteData.hpp"
#include "../../Core/Util/Threading/LongTaskHelp.h"
#include "../../Core/Util/Threading/AutoResetEvent.h"
#include "../../Core/Util/Concurrent/ConcurrentQueue.h"

#include "../../Core/Module/Log/Logger.hpp"

namespace Sers{


class SocketConnect
{

private:

	//will be called when disconnected
	void(*OnDisConnected)();

public:




	SocketConnect():OnDisConnected(nullptr)
	{

	}
	~SocketConnect()
	{
		Close();
	}

	//will be called when disconnected
	void OnDisConnected_Set(void(*OnDisConnected)())
	{
		this->OnDisConnected=OnDisConnected;
	}

	bool IsConnected()
	{
		return socketState==1;
	}


	bool Init(int workThreadCount,const char* serverAddr,int serverPort)
	{

		if(IsConnected()) return false;

		//(x.1) connect socket
		Logger::Info("[SersMq Socket Client]try connect socket.");
		if(0!=_Socket_Connect(serverAddr,serverPort)){
			return false;
		}

		//(x.2) init back task

		taskToReceiveMsg.Action_Set(&TaskToReceiveMsg,this);
		taskToReceiveMsg.Start();

		taskToSendMsg.Action_Set(&TaskToSendMsg,this);
		taskToSendMsg.Start();


		taskToDealMsg.ThreadCount_Set(workThreadCount);
		taskToDealMsg.Action_Set(&TaskToDealMsg,this);
		taskToDealMsg.Start();

		return true;
	}




	void Close()
	{
		if(!IsConnected()) return;

		//(x.1) stop thread
		taskToSendMsg.Stop_Temp();
		taskToReceiveMsg.Stop_Temp();
		taskToDealMsg.Stop_Temp();

	   // (x.2) close socket
	   _Socket_Close();

	   if(OnDisConnected)
	   {
		   OnDisConnected();
	   }

	}


	void OnGetMsg_Set(void (*onGetMsg)(SocketConnect * mq,const ArraySegment& data,void * arg),void * arg)
	{
		onGetMsg_arg=arg;
		this->onGetMsg=onGetMsg;
	}



	//return true if success
	bool SendMsg(ByteData& data)
	{
		int len=data.Length();
		ArraySegment arr;
		arr.CopyFrom((char *) &len ,4);
		data.InsertData(arr);


		ArraySegment dataBytes = data.ToArraySegment();

		msgToSend.EnQueue(dataBytes);
		autoResetEvent_OnGetMsg.Set();
		return true;
	}



private:

	// 0:not connect     1: connected            2:disconnected
	int socketState;

	std::mutex  sendMsgLock;

	int socket_descriptor;

	void (*onGetMsg)(SocketConnect * mq,const ArraySegment& data,void * arg) ;
    void * onGetMsg_arg;


	//--[[  (x.1)后台接收消息logic


	LongTaskHelp taskToReceiveMsg;
	AutoResetEvent autoResetEvent_OnReveiveMsg;

	LongTaskHelp taskToDealMsg ;
	ConcurrentQueue<ArraySegment> msgToDeal;

	LongTaskHelp taskToSendMsg;
	ConcurrentQueue<ArraySegment> msgToSend;
	AutoResetEvent autoResetEvent_OnGetMsg;





	//----[[  (x.1.2)后台接收消息线程 TaskToReceiveMsg

	static void TaskToDealMsg(void*lp)
	{
		SocketConnect* self=(SocketConnect*)lp;
		ArraySegment msg;
		while (true)
		{

			if(self->msgToDeal.TryDeQueue(msg) )
			{
				if(msg.len>=0)
				{
					self->onGetMsg(self,msg,self->onGetMsg_arg);
				}
			}
			else
			{
				self->autoResetEvent_OnReveiveMsg.WaitOne(10);
			}
		}
	}

	/////////////////////////////////////////////////////////////////////
	static void TaskToSendMsg(void*lp)
		{
			SocketConnect* self=(SocketConnect*)lp;
			ArraySegment msg;
			while (self->IsConnected())
			while (true)
			{

				if(self->msgToSend.TryDeQueue(msg) )
				{
					if(!self->_Socket_SendData(msg.GetData(),msg.len))
					{
						return ;
					}
				}
				else
				{
					self->autoResetEvent_OnGetMsg.WaitOne(10);
				    break;
				}
			}

		}




	//----]]  (x.1.2)后台接收消息线程 TaskToReceiveMsg



	//----[[  (x.1.2)后台接收消息线程 TaskToReceiveMsg

	static void TaskToReceiveMsg(void * lp)
	{
		SocketConnect* self=(SocketConnect*)lp;
		ArraySegment msg;
		while (self->IsConnected())
		{
			self->_Socket_ReadMsg(msg);

			if(msg.len<=0) {
				Logger::Error("[SersMq Socket Client]socket can't read data.try close socket.");
				self->Close();
				return;
			}

			//CacheReceivedMsg
			self->msgToDeal.EnQueue(msg);
			self->autoResetEvent_OnReveiveMsg.Set();

		}

	}
	//----]]  (x.1.2)后台接收消息线程 TaskToReceiveMsg



	//--]]  (x.1)后台接收消息logic




	// Layout socket

	//return 1 if success
	bool  _Socket_SendData(char * str,int len)
	{
		if(send(socket_descriptor,str,len,0) == -1)
		{
			return false;
		}
		return true;
	}



	// return  len         -1 -2 -3:fail       other:the length of data
	void  _Socket_ReadMsg(ArraySegment & msg)
	{
		int curRead;

		//(x.1)get len
		int len;
		curRead=recv(socket_descriptor,&len,4,0);
		if(curRead<=0)
		{
			msg.len=-1;
			return ;
		}
		if(len<=0){
			msg.len=len;
		    return;
		}

		//(x.2) get data
		msg.Malloc(len);
		msg.len=len;
		char * bytes=msg.GetData();
//		if(bytes==0)
//		{
//			msg.len=-2;
//		    return ;
//		}

		int readedCount=0;

		for(;readedCount<len;){
			curRead=recv(socket_descriptor,bytes+readedCount,len-readedCount,0);
			if(curRead<=0){
				msg.len=-3;
			    return;
			}
			readedCount+=curRead;
		}

	}





	void _Socket_Close()
	{
		if(socketState==1)
		{
		  close(socket_descriptor);
		  socketState=2;
		}
	}




	//return 0  :success
	int _Socket_Connect(const char * serverAddr,int serverPort)
	{
		struct sockaddr_in pin;

		struct hostent *server_host_name;


		if((server_host_name = gethostbyname(serverAddr))==0)
		{
			Logger::Error("[SersMq Socket Client]Error resolving local host ");
			return 1;
		}

		memset(&pin,0,sizeof(pin));
		pin.sin_family =AF_INET;

		pin.sin_addr.s_addr=htonl(INADDR_ANY);

		pin.sin_addr.s_addr=((struct in_addr *)(server_host_name->h_addr))->s_addr;

		pin.sin_port=htons(serverPort);

		if((socket_descriptor =socket(AF_INET,SOCK_STREAM,0))==-1)
		{
			Logger::Error("[SersMq Socket Client]Error opening socket ");
			return 2;
		}

		if(connect(socket_descriptor,(struct sockaddr *)&pin,sizeof(pin))==-1)
		{
			Logger::Error("[SersMq Socket Client]Error connecting to socket ");
			return 3;
		}

		socketState=1;
		return 0;
	}









};


}

#endif /* SERS_MQ_SOCKETMQ_SOCKETCONNECT_H_ */
