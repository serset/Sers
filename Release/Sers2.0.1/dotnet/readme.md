# Sers NetCore 部署文件（Sers）

可部署到windows或Linux(centos7或ubuntu) 。
务必先运行服务中心（ServiceCenter）
>运行环境 [netcore2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) 

[windows点我下载netcore2.1 runtime](https://download.visualstudio.microsoft.com/download/pr/c551fea4-c065-4142-9556-4d78fb949284/efe7c2ef2d51331bd0fced6ea0eadf08/dotnet-runtime-2.1.8-win-x64.exe)

# 运行步骤

   (x.1)下载[程序文件](https://raw.githubusercontent.com/sersms/Sers/2.0.1/release/Release/Sers2.0.1/dotnet/Sers-v2.0.1.zip)
   
   (x.2) 解压zip文件，windows系统依次双击bat文件，Linux执行bash.txt中的命令即可。
   
   (x.3) 打开服务治理导航页面 [http://localhost:6022/index.html](http://localhost:6022/index.html)

# 性能初测
>qps数据来源于jmeter和 http://ip:6022/ApiStationMng.html 页面中的统计


项目均部署在同一机器,消息队列使用SocketMq，调用线程数10,Sers1版本性能
   
| Os  | qps(内部调用)  | qps(http网关调用)  |qps(jmeter调用http网关)  |
| ------------ | ------------ | ------------ |------------ |
|  Ubuntu(6核2G) | 7000  | 2000 | 1700 |
|  CentOs7(1核1G) | 4000  | 1100 | 840 |
|  CentOs7(2核1G) | 5000  | 1500-2000 | 1300|
|  CentOs7(6核1G) | 8000-9400  | 3000 | 1800 |
| Windows10| 6000 | 400| 1600 |
| Server2012(6核2G)| 15000-17000 |  | 2200 |


 
 










