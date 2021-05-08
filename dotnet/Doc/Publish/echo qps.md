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

http://127.0.0.1:4580/_gover_/index.html?user=admin_123456




#----------------------------------------------
# sers单体压测(net6.0)

CentOs8(2x24核) .net6


方式 线程数（处理/请求）	qps（cpu利用率）
type workThread/requestThread	qps（cpu利用率）


ApiClientAsync 16/16	140-150万（15%）   

ApiClientAsync 18/18	150-180万（18%） 

ApiClientAsync 20/20	160-230万（18%）
   
ApiClientAsync 22/22	160-190万（19%）	

ApiClientAsync 24/24	160-180万（21%）   




