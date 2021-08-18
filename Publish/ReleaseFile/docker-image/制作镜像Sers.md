#构建多架构镜像

#docker login -u serset -p xxxxxxxxx

#---------------------------------------------------------------------
#(x.1)初始化构建器

#启用 buildx 插件
export DOCKER_CLI_EXPERIMENTAL=enabled

#验证是否开启
docker buildx version

#启用 binfmt_misc
docker run --rm --privileged docker/binfmt:66f9012c56a8316f9244ffd7622d7c21c1f6f28d

#验证是 binfmt_misc 否开启
ls -al /proc/sys/fs/binfmt_misc/


#创建一个新的构建器
docker buildx create --use --name mybuilder

#启动构建器
docker buildx inspect mybuilder --bootstrap

#查看当前使用的构建器及构建器支持的 CPU 架构，可以看到支持很多 CPU 架构：
docker buildx ls



#---------------------------------------------------------------------
#(x.2)构建多架构镜像（ arm、arm64 和 amd64 ）并推送到 Docker Hub

#把文件夹拷贝到image下
cd /root/image


#构建镜像并推送到 Docker Hub 
export tag=2.1.9

cd sers
docker buildx build . -t serset/sers:${tag} -t serset/sers --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
cd .. 


cd sers-gover 
docker buildx build . -t serset/sers-gover:${tag} -t serset/sers-gover --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
cd ..


cd sers-gateway
docker buildx build . -t serset/sers-gateway:${tag} -t serset/sers-gateway --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
cd ..



cd sers-demo-robot 
docker buildx build . -t serset/sers-demo-robot:${tag} -t serset/sers-demo-robot --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
cd ..

cd sers-demo
docker buildx build . -t serset/sers-demo:${tag} -t serset/sers-demo --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
cd ..

cd sers-demo-sersall 
docker buildx build . -t serset/sers-demo-sersall:${tag} -t serset/sers-demo-sersall --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
cd ..

 


#强制删除镜像名称中包含sers的镜像
# docker rmi --force $(docker images | grep sers | awk '{print $3}')












 