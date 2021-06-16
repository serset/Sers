## (1)MessageFrame(Mq)
>只负责消息帧传递，不负责其他功能

### (x.1)IMqConn

#### (x.x.1)state
	连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
#### (x.x.2)connTag
#### (x.x.3)SendFrameAsync
#### (x.x.4)Close


### (x.2)ServerMq:

Start
Stop
ConnectedList
Conn_OnConnected
Conn_OnDisconnected
Conn_OnGetFrame 


### (x.3)ClientMq:
 
Connect
Close
mqConn
Conn_OnDisconnected 
OnGetFrame
secretKey



## (2)MqManager

### (x.1)ServerMqManager

Start
Stop

Conn_OnConnected
Conn_OnDisconnected

station_OnGetRequest
station_OnGetMessage

Station_SendRequestAsync
Station_SendMessageAsync



### (x.2)ClientMqManager

Start
Stop

mqConns 
Conn_OnDisconnected

station_OnGetRequest
station_OnGetMessage
  
Station_SendRequest
Station_SendMessageAsync


### (x.3)RequestAdaptor
 请求解析，缓存，转发，心跳包检测 



## (3)Service

### (x.1)ServerCenter

#### (x.x.1)actionsOnStart actionsOnStop
#### (x.x.2)mqMng
		线程数：Sers.Mq.Config.workThreadCount
#### (x.x.3)localApiService
		线程数：Sers.LocalApiService.workThreadCount
#### (x.x.4)ApiClient
#### (x.x.5)MessageClient
#### (x.x.6)ApiCenterService
		线程数：0。任务都在mqMng线程中完成
#### (x.x.7)MessageCenterService



### (x.2)ServerStation

#### (x.x.1)actionsOnStart actionsOnStop
#### (x.x.2)mqMng
		线程数：Sers.Mq.Config.workThreadCount
#### (x.x.3)localApiService
		线程数：Sers.LocalApiService.workThreadCount
#### (x.x.4)ApiClient
#### (x.x.5)MessageClient

 











