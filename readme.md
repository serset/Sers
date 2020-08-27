﻿# Sers微服务架构协议（Sers 2.1.1/release）

Sers为一套跨平台跨语言的开源微服务架构协议。
● 跨语言，目前已支持c#、java、c++、javascript。
● 轻量简洁，javascript接入代码不到1000行，压缩后只有8KB。您可以在浏览器上通过javascript接入提供api服务。
● 高效高并发，.net core版本（2.1.1）单机QPS可稳定在15万以上。
● 高扩展性，可以自行扩展接入。
● 支持tcp、zmp、websocket、ipc等多种通讯方式。
● 可以非侵入式接入.net core web api，接入代码只有1行。


源码地址：https://github.com/sersms/Sers

此版本为v2.1.1版。(RequestQueue模式)


# 部署
参见[Release-Sers-v2.1.1](https://github.com/sersms/Sers/tree/2.1.1/release/Release/Sers2.1.1/netcore)

[点我查看docker部署](https://github.com/sersms/Sers/tree/2.1.1/release/Release/Sers2.1.1/netcore/docker)


# 性能测试
>部署（或运行）ServiceCenter Gover Gateway Robot StationDemo五个项目。


>qps数据来源于 ab 和 http://ip:6022/ApiStationMng.html 页面中的统计

项目部署在同一机器, Robot和StationDemo都附加在ServiceCenter上
   
| Os  |  robot内部调用(8线程)   |  ab压测(32线程，CGateway)   |
| ------------ | ------------ | ------------ |
| Windows10 |  150000 | |
| CentOs7(2核1G) |15000|3500|
| CentOs7(4核1G) |25000|5000|
 



# 模块

## (x.1)服务中心(ServiceCenter)
>功能：服务注册，请求分发（负载均衡），消息订阅等。所有服务站点都需要向此站点注册。


## (x.2)服务治理(Gover)
>功能：管理监控服务、站点，服务限流，服务统计等。服务治理实际部署在服务中心，此站点为服务治理的用户界面。

部署后可打开地址进行管理 http://ip:6022/index.html


  
## (x.3)Http网关(ServiceStation)
>功能：外部接口通过本网关调用内部服务。

网关有两个版本，c++版（CGateway）和dotnet版(Gateway)， c++版（CGateway）相对更高效。


## (x.4)服务站点(ServiceStation)

### (x.x.1)StationDemo
>功能：站点Demo。

dotnet\netcore\Station\StationDemo\App.StationDemo.Station

### (x.x.2)Robot
>功能：可以循环调用指定接口，可用来负载测试。

dotnet\netcore\Station\Robot\App.Robot.Station
http://ip:6022/_robot_/TaskMng.html
 
 
