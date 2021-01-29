
当前为Sers2版本， RequestQueue模式
--------------------------------------------


# Sers微服务架构协议（Sers v2.1.1）
Sers为一套 跨平台 跨语言 的开源 微服务架构协议。

# 当前项目为netcore版本
>开发环境 [netcore2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) 


# 部署
参见[Sers_NetCore_HelloWorld_Publish](https://github.com/serset/Sers_NetCore_HelloWorld_Publish/tree/master/Sers/Latest)


# 性能初测
>部署（或运行）ServiceCenter Gover Gateway Robot StationDemo五个项目。

>qps数据来源于jmeter和 http://ip:6022/ApiStationMng.html 页面中的统计


项目均部署在同一机器,消息队列使用SocketMq
   
| Os  | 调用线程数 | qps(内部调用)  | qps(http网关调用-本机)  |qps(jmeter调用http网关-外部)  |
| ------------ | ------------ | ------------ | ------------ |------------ |
|  Ubuntu(6核2G) | 10 | 7000  | 2000 | 1700 |
|  CentOs7(1核1G) | 10 | 4000  | 1100 | 840 |
|  CentOs7(2核1G) | 10 | 5000  | 1500-2000 | 1300|
|  CentOs7(6核1G) | 10 | 8000-9400  | 3000 | 1800 |
| Windows10| 10 | 6000 | 400| 1600 |
| Server2012(6核2G)| 10 | 15000-17000 |  | 2200 |



# 模块

## (x.1)消息队列Mq
 Sers\Sers.Mq


## (x.2)服务中心(ServiceCenter)
  App\ServiceCenter\App.ServiceCenter


## (x.3)服务治理(Gover)
  App\ServiceCenter\Gover\UI\App.Gover.Gateway
  
  http://localhost:6022/index.html



  
## (x.4)网关(ServiceStation)
  App\Gateway\App.Gateway




## (x.5)服务站点(ServiceStation)
  App\Station\StationDemo\App.Station.StationDemo
  
  App\Station\Robot\App.Station.Robot
  
  App\Station\AuthCenter\App.Station.AuthCenter










