#--------------------------------
qps 160万 
no sleep
1  server 
20 client thead:1000   msgLen:1
#--------------------------------


#启动服务端
dotnet /root/app/DeliveryServer/DeliveryServer.dll


#启动客户端
dotnet /root/app/DeliveryClient/DeliveryClient.dll 192.168.10.10 4501 20000 150 


#后台启动客户端
dotnet /root/app/DeliveryClient/DeliveryClient.dll 192.168.10.10 4501 10000 150 > /root/app/console.log 2>&1 &



#杀死所有客户端
kill -s 9 `pgrep -f 'DeliveryClient.dll'`




#--------------------------------
启动20个进程
dotnet DeliveryClient.dll 192.168.10.11 4501 300 102 > console.log 2>&1 &

qps为 三百万


#--------------------------------
启动20个进程
dotnet DeliveryClient.dll 192.168.10.11 4501 300 512 > console.log 2>&1 &

qps为 1百万

实时网速为   读:500MB/s  写:500MB/s



#------------------------------------
centos8 ThreadWait
thread = 10000*2		msgLen = 150       qps = 180万





#------------------------------------
windows ThreadWait
thread = 40000		msgLen = 1       qps = 117万


#------------------------------------
windows Timer
thread = 40000		msgLen = 1       qps = 116万










