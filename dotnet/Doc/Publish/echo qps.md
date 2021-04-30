#发送获取qps请求
curl -H "Cookie: user=admin_123456" http://192.168.56.1:4580/_gover_/serviceCenter/statistics


#显示qps
curl -s -H "Cookie: user=admin_123456" http://192.168.56.1:4580/_gover_/serviceCenter/statistics | grep -Eo '[0-9|\.]+'


#每秒显示一次qps
for i in {1..10}   
do  
curl -s -H "Cookie: user=admin_123456" http://192.168.56.1:4580/_gover_/serviceCenter/statistics | grep -Eo '[0-9|\.]+'
sleep 1
done  
 

 
 