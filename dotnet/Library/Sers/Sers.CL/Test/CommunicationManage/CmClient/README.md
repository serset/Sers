

1 server 
20 client 1000thead   qps 160万
 




cd /root/app
dotnet CmServer/CmServer.dll


cd /root/app
dotnet CmClient/CmClient.dll > console.log 2>&1 &


#杀死mc用户端 
kill -s 9 `pgrep -f 'CmClient.dll'`

#--------------------------------
#--------------------------------

dotnet CLClient.dll > console.log 2>&1 &

启动10个进程的qps为 10万

启动1个进程的qps是4万


 