#导出镜像
tag=2.1.12-temp

docker save -o /root/image/dotnet.2.1.tar serset/dotnet:2.1
docker save -o /root/image/sers-cgateway-1.2.0.tar serset/sers-cgateway:1.2.0


docker save -o /root/image/sers.${tag}.tar serset/sers:${tag}
docker save -o /root/image/sers-gateway.${tag}.tar serset/sers-gateway:${tag}
docker save -o /root/image/sers-gover.${tag}.tar serset/sers-gover:${tag}

docker save -o /root/image/sers-demo-robot.${tag}.tar serset/sers-demo-robot:${tag}
docker save -o /root/image/sers-demo-sersall.${tag}.tar serset/sers-demo-sersall:${tag}
docker save -o /root/image/sers-demo.${tag}.tar serset/sers-demo:${tag}



#导入镜像

docker load -i /root/image/dotnet.2.1.tar
docker load -i /root/image/sers-cgateway.1.2.0.tar

docker load -i /root/image/sers.${tag}.tar
docker load -i /root/image/sers-gateway.${tag}.tar
docker load -i /root/image/sers-gover.${tag}.tar

docker load -i /root/image/sers-demo-robot.${tag}.tar
docker load -i /root/image/sers-demo-sersall.${tag}.tar 
docker load -i /root/image/sers-demo.${tag}.tar 


 