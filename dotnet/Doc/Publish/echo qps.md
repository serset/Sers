#发送获取qps请求
curl -H "Cookie: user=admin_123456" http://localhost:4580/_gover_/serviceCenter/statistics


#显示qps
curl -s -H "Cookie: user=admin_123456" http://localhost:4580/_gover_/serviceCenter/statistics | grep -Eo '[0-9|\.]+'


#每3秒显示一次qps
for i in {1..10}   
do  
curl -s -H "Cookie: user=admin_123456" http://localhost:4580/_gover_/serviceCenter/statistics | grep -Eo '[0-9|\.]+'
sleep 3
done  

#每3秒显示一次qps
for i in {1..100}; do curl -s -H "Cookie: user=admin_123456" http://localhost:4580/_gover_/serviceCenter/statistics | grep -Eo '[0-9|\.]+'; sleep 3; done 





 dotnet /root/app/ServiceCenter/App.ServiceCenter.dll
 

 http://lanxing.cloud:4580/_gover_/index.html?user=admin_123456



 # qps: 150万	workThread: 16	requestThread: 16 

 # qps:		workThread: 24	requestThread: 24 


方式 线程数（处理/请求）     qps
workThread/requestThread qps

CentOs8(2x24核) .net6

ApiClientAsync 16/16	140-150万（15%）   
ApiClientAsync 24/24	160-180万（21%）   

   
ApiClientAsync 22/22	160-190万（19%）	 

ApiClientAsync 20/20	160-230万（18%）

ApiClientAsync 18/18	150-180万（18%）
