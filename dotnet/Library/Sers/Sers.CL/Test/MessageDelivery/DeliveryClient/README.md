

1 server 
20 client 1000thead   qps 160万

cd /root/app
dotnet DeliveryServer/DeliveryServer.dll




cd /root/app
dotnet DeliveryClient/DeliveryClient.dll 127.0.0.1 4501 100 1



dotnet DeliveryClient/DeliveryClient.dll 127.0.0.1 4501 100 1 > console.log 2>&1 &


#杀死mc用户端 
kill -s 9 `pgrep -f 'DeliveryClient.dll'`

#--------------------------------
 
dotnet DeliveryClient.dll 192.168.10.11 4501 200 1

qps为 7.3万
cpu  92%






cpu  87%











#--------------------------------
启动20个进程
dotnet DeliveryClient.dll 192.168.10.11 4501 300 102 > console.log 2>&1 &

qps为 三百万


#--------------------------------
启动20个进程
dotnet DeliveryClient.dll 192.168.10.11 4501 300 512 > console.log 2>&1 &

qps为 1百万

实时网速为   读:500MB/s  写:500MB/s