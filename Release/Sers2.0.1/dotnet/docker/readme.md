# Sers v2.0.1 docker部署

## (x.1)安装docker 

## (x.2)部署ServiceCenter
>会监听端口10345

//1.下载镜像
docker pull sersms/sers_dotnet_servicecenter:2.0.1

//2.创建容器并运行
docker run --name=sers_servicecenter --net=host -d -v /etc/localtime:/etc/localtime sersms/sers_dotnet_servicecenter:2.0.1



## (x.3)部署Gover
>会开启端口6022提供http服务

//1.下载镜像
docker pull sersms/sers_dotnet_gover:2.0.1

//2.创建容器并运行
docker run --name=sers_gover --net=host -d -v /etc/localtime:/etc/localtime sersms/sers_dotnet_gover:2.0.1

//3.打开gover ui页面
http://ip:6022/index.html



## (x.5)部署Robot和StationDemo(演示站点)

//1.下载镜像
docker pull sersms/sers_dotnet_robot:2.0.1
docker pull sersms/sers_dotnet_stationdemo:2.0.1

//2.创建容器并运行
docker run --name=sers_robot --net=host -d -v /etc/localtime:/etc/localtime sersms/sers_dotnet_robot:2.0.1
docker run --name=sers_stationdemo --net=host -d -v /etc/localtime:/etc/localtime sersms/sers_dotnet_stationdemo:2.0.1
