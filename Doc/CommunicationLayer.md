## MqManager

### (x.1)Frame数据帧格式
第1部分： 消息模式 EFrameType         1 byte
第2部分： 请求类别 ERequestType     1 byte
第3部分： 消息内容

### (x.2)消息块类型
消息块有3类(EFrameType)

	1.request
	//带回应消息中的请求

	2.reply
	//带回应消息中的回应

	3.message
	//无回应消息的消息内容



### (x.3)模式种类

#### (x.x.1) 带回应请求（ReqRep模式）

>使用ReqRepFrame格式数据
//ReqRepFrame 消息帧(byte[])	 
第1部分： 请求标识（reqKey）(long)			长度为8字节
第2部分： 消息内容(oriMsg)


内部又分2类
##### (x.x.x.1) heartBeat
>连接心跳。RequestType为heartBeat
>请求内容和回应内容都为版本号

##### （x.x.x.2) app
>例如api调用。RequestType为app 

##### (x.x.x.3)ERequestType枚举说明 （byte）
	0: app
	1: heartBeat



### (x.x.2) Message
>无回应消息（单向消息）