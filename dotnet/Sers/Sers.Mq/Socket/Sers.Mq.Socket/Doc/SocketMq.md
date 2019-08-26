## (1)Layout1:	SocketConnect
>tcp socket底层。内部有两层封装。

### (x.1)第一层封装
>提供收发单个消息块接口
ReadMsg
WriteMsg

消息块格式：
	第一部分(len)    数据长度，4字节 Int32类型
	第二部分(data)   原始数据，长度由第二部分指定


### (x.2)第二层封装
>有两个后台线程：收消息线程、发消息线程
对外提供收发单个消息块接口

OnGetMsg
SendMsg

## (2)Layout2:	MqConnect
底层消息格式为：

		//Data数据帧
		第1部分： 消息模式 MsgType         1 byte
		第2部分： 请求类别 RequestType     1 byte
		第3部分： 消息内容

### (x.1)模式种类
>提供两类模式

1. 带回应消息（Request模式）
2. 无回应消息（单向消息模式Message）


带回应消息使用ReqRepFrame格式数据

	//ReqRepFrame 消息帧(byte[])	 
	第1部分： 请求标识（reqKey）(long)			长度为8字节
	第2部分： 消息内容(oriMsg)


### (x.2)消息块类型
消息块有3类(EMsgType)

	1.request
	//带回应消息中的请求

	2.reply
	//带回应消息中的回应

	3.message
	//无回应消息的消息内容



### (x.3)模式详解
#### (x.x.1) 带回应消息（Request模式）
内部又分2类

##### (x.x.x.1)Ping
>连接心跳。RequestType为ping
提供发送Ping请求接口
请求内容和回应内容都为版本号

##### （x.x.x.2) ReqRep
>请求回应模式，例如api调用。RequestType为app
提供 1 发送请求 ，2 收到的请求的回调



##### (x.x.x.3)ERequestType枚举说明 （byte）
	0: app
	1: ping



### (x.x.2) Message
>无回应消息（单向消息）
提供 1 发送消息， 2 收到的消息的回调

 


## (3)Layout3:	ServerMq ClientMq
>在第二层的基础上进行封装

### (x.1) ReqRep模式
>双向请求回应

### (x.2) Message
>无回应消息（单向消息）
提供 1 发送消息， 2 收到的消息的回调

### (x.3) PingThread
>连接成功后，后台自动创建心跳测试线程
