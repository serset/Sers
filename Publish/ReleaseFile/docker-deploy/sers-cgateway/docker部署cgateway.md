﻿#docker部署sers-cgateway


---------------------------------
#(x.1)文件
  (x.1)把本文件所在目录中所有文件拷贝到宿主机
  (x.2)修改配置文件 appsettings.json
 


#(x.2)创建容器并运行
(--name 容器名称，可自定义)
(--restart=always 自动重启)
(-v /etc/localtime:/etc/localtime)挂载宿主机localtime文件解决容器时间与主机时区不一致的问题
(-v $PWD/data:/data 将主机中当前目录下的data挂载到容器的/data)
(--net=host 网络直接使用宿主机网络)（-p 6022:6022 端口映射）

cd /root/docker

cd sers-cgateway
docker run --name=sers-cgateway --restart=always -d \
-p 6008:6008 \
-v /etc/localtime:/etc/localtime \
-v $PWD/appsettings.json:/root/app/appsettings.json \
-v $PWD/Logs:/root/app/Logs \
serset/sers-cgateway:1.2.0
cd ..


#端口    http://ip:6008


#(x.3)应用已经运行
   可在文件夹Logs 中查看日志

-------------------
#常用命令

#查看容器logs
docker logs sers-cgateway

#在容器内执行命令行
docker  exec -it sers-cgateway bash

#停止容器
docker stop sers-cgateway

#打开容器
docker start sers-cgateway

#重启容器
docker restart sers-cgateway


#删除容器
docker rm sers-cgateway -f


