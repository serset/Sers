# Sers简介
Sers为一套跨平台跨语言的开源微服务架构协议
>源码地址：[https://github.com/serset/Sers](https://github.com/serset/Sers "https://github.com/serset/Sers")  
>当前版本为2.1，RequestQueue模式。  

Sers拥有如下特性：  
● 跨语言、跨平台，目前已支持c#、java、c++、javascript  
● 高效高并发（百万并发），单机QPS:2000000  
● 轻量简洁，可javascript接入，代码不到1000行，压缩后只有8KB  
● 易扩展，可以自行扩展接入  
● 支持IOCP、ZMQ、WebSocket、NamedPipe、SharedMemory等多种通讯方式  
● 无代码侵入，.net core接入代码只有1行  



# 站点划分
 Sers为中心化的微服务架构协议，按照身份分为服务中心和服务站点。

## (x.1)服务中心
　　服务中心(ServiceCenter)提供服务注册、服务发现、请求分发（负载均衡）、Api站点管理、服务站点管理、消息订阅等等服务。  
　　所有服务站点都需要向此服务中心进行注册。所有的请求都会经过服务中心进行转发。  
　　服务中心内置Gover服务治理功能。提供服务管理监控，站点管理监控，服务限流，服务统计等功能。服务治理部署在服务中心。  
　　服务治理的入口地址为　http://ip:6022/_gover_/index.html  
　　端口号在appsettings.json配置文件中配置。  


## (x.2)服务站点
　　服务站点(ServiceStation)提供应用层服务。  
　　服务站点可以有多个，通过服务中心相互连接。在服务站点启动时，主动向服务中心发起服务注册请求，注册服务。  
　　服务站点注册成功后即可向其他站点（包含服务网关）提供服务。可调用其他站点提供的服务。  
　　提供的服务以url(route)作为服务标识。  
　　可以把服务站点直接附加到服务中心（免除通信层，单体模式）提供服务。200百万qps的性能数据就是在此模式下检测所得。  

## (x.3)服务网关
　　服务网关(Gateway)通过http方式对外暴露内部服务。  
　　服务网关是一个特殊的服务站点。网关用http监听请求，把请求转发到服务中心。服务网关为服务的对外入口。  
　　服务中心内置了网关，可以在appsettings.json配置文件中配置进行启用。  
　　网关有两个版本，c++版（CGateway）和dotnet版(Gateway)。 c++版（CGateway）相对更高效。  
