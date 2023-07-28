# docker部署sers-servicecenter



---------------------------------
# (x.1)文件
  (x.1)把本文件所在目录中所有文件拷贝到宿主机
  (x.2)修改配置文件 appsettings.json
 


# (x.2)创建容器并运行
``` bash
cd /root/docker

cd sers
docker run --name=sers --restart=always -d \
-p 4580:4580 -p 4501:4501 \
-v /etc/localtime:/etc/localtime \
-v $PWD/appsettings.json:/root/app/appsettings.json \
-v $PWD/Logs:/root/app/Logs \
-v $PWD/Data:/root/app/Data \
serset/sers
cd ..


# 精简
docker run --name=sers --restart=always -d -p 4580:4580 -p 4501:4501 serset/sers

gover     http://ip:4580
通信端口 tcp://ip:4501

```


# (x.3)应用已经运行
   可在文件夹ServiceCenter/Logs 中查看日志


#---------------------------------------
# 常用命令

# 查看容器logs
docker logs sers

# 在容器内执行命令行
docker  exec -it sers bash

# 停止容器
docker stop sers

# 打开容器
docker start sers

# 重启容器
docker restart sers


# 删除容器
docker rm sers -f


