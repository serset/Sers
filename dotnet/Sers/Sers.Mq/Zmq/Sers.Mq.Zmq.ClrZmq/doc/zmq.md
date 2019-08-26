## (1)Layout1:	SocketConnect
> zmq底层。提供收发单个消息块接口

OnGetMsg
SendMsg


## (2)Layout2:	ServerMq ClientMq
### (x.1)模式种类
>提供两类模式

1. 带回应消息（Request模式）
2. 无回应消息（单向消息模式Message）


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


