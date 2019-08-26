# Sers NetCore 部署文件（Sers）

可部署到windows或Linux(centos7或ubuntu) 。
务必先运行服务中心（ServiceCenter）
>运行环境 [netcore2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) 

[windows点我下载netcore2.1 runtime](https://download.visualstudio.microsoft.com/download/pr/c551fea4-c065-4142-9556-4d78fb949284/efe7c2ef2d51331bd0fced6ea0eadf08/dotnet-runtime-2.1.8-win-x64.exe)

# 运行步骤

   (x.1)下载[程序文件](https://raw.githubusercontent.com/sersms/Sers_NetCore_HelloWorld_Publish/master/Sers/Latest/Sers-Publish-Latest.zip)
   
   (x.2) 解压zip文件，windows系统双击Startup-All.bat，Linux执行Startup-All.bash中的命令即可。
   
   (x.3) 打开服务治理导航页面 [http://localhost:6022/index.html](http://localhost:6022/index.html)

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


## (x.1)服务中心(ServiceCenter)
>功能：服务注册，请求分发（负载均衡），消息订阅等。所有服务站点都需要向此站点注册。



## (x.2)服务治理(Gover)
>功能：管理监控服务、站点，服务限流，服务统计等。服务治理实际部署在服务中心，此站点为服务治理的用户界面。
部署后可打开地址进行管理 http://ip:6022/index.html


  
## (x.3)Http网关(ServiceStation)
>功能：外部接口通过本网关调用内部服务。


## (x.4)服务站点(ServiceStation)

### StationDemo
>功能：站点Demo。



### Robot
>功能：可以循环调用指定接口，可用来负载测试。
http://ip:6022/_robot_/TaskMng.html

 










