# Sers v2.0.1 docker部署

## (x.1)安装docker

## (x.2)修改 daemon.json
>需要https，我们可以修改下daemon.json来解决：
### (x.1)修改文件 /etc/docker/daemon.json 
```javascript
{
  "registry-mirrors": [ "http://hub-mirror.c.163.com"],
  "insecure-registries": [ "sersms.com:17005"]   //添加此行
}
```

### (x.2)重启docker

systemctl  restart docker



## (x.3)部署ServiceCenter
>会监听端口10345

//1.下载镜像

docker pull sersms.com:17005/sers/dotnet/servicecenter:v2.0.1

//2.创建容器并运行

docker run --name=sers_servicecenter --net=host -d -v /etc/localtime:/etc/localtime sersms.com:17005/sers/dotnet/servicecenter:v2.0.1



## (x.4)部署Gover
>会开启端口6022提供http服务

//1.下载镜像

docker pull sersms.com:17005/sers/dotnet/gover:v2.0.1

//2.创建容器并运行

docker run --name=sers_gover --net=host -d -v /etc/localtime:/etc/localtime sersms.com:17005/sers/dotnet/gover:v2.0.1

//3.打开gover ui页面

http://ip:6022/index.html



## (x.5)部署Robot和StationDemo(演示站点)

//1.下载镜像

docker pull sersms.com:17005/sers/dotnet/robot:v2.0.1

docker pull sersms.com:17005/sers/dotnet/stationdemo:v2.0.1

//2.创建容器并运行

docker run --name=sers_robot --net=host -d -v /etc/localtime:/etc/localtime sersms.com:17005/sers/dotnet/robot:v2.0.1

docker run --name=sers_stationdemo --net=host -d -v /etc/localtime:/etc/localtime sersms.com:17005/sers/dotnet/stationdemo:v2.0.1