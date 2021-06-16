

 


 
dotnet /root/app/CmServer/CmServer.dll


dotnet /root/app/CmClient/CmClient.dll > console.log 2>&1 &


#杀死进程
kill -s 9 `pgrep -f 'CmClient.dll'`
 
#--------------------------------

dotnet CLClient.dll > console.log 2>&1 &

启动10个进程的qps为 10万

启动1个进程的qps是4万

 
#--------------------------------
# centos8
1server;  2client; 4000 thead;	mode: Timer ;      msgLen:10	qps: 98万

1server;  2client; 1000 thead;	mode: Timer ;      msgLen:100	qps: 84万