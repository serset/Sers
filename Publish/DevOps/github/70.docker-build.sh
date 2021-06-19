set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn

export version=`grep '<Version>' "${codePath}" -r --include Sers.Core.csproj | grep -oP '>(.*)<' | tr -d '<>'`

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

# "

 
netVersion=net6.0

#---------------------------------------------------------------------
echo "(x.2)copy SersDocker"

releasePath=$codePath/Publish/release/Sers
appPath=$codePath/Publish/release/Sers/SersPublish/$netVersion

echo "copy SersDocker"
cp -rf "$codePath/Publish/PublishFile/SersDocker/." "$releasePath"

echo "copy sers"
cp -rf "$appPath/ServiceCenter/appsettings.json" "$releasePath/docker部署Sers/sers"
cp -rf "$appPath/ServiceCenter/." "$releasePath/docker制作镜像Sers/sers/app"


echo "copy sers-gateway"
cp -rf "$appPath/Gateway/appsettings.json" "$releasePath/docker部署Sers/sers-gateway"
cp -rf "$appPath/Gateway/." "$releasePath/docker制作镜像Sers/sers-gateway/app"

echo "copy sers-gover"
cp -rf "$appPath/Gover/appsettings.json" "$releasePath/docker部署Sers/sers-gover"
cp -rf "$appPath/Gover/." "$releasePath/docker制作镜像Sers/sers-gover/app"



echo "copy sers-demo"
cp -rf "$appPath/Demo/appsettings.json" "$releasePath/docker部署Sers/sers-demo"
cp -rf "$appPath/Demo/." "$releasePath/docker制作镜像Sers/sers-demo/app" 

echo "copy sers-demo-robot"
cp -rf "$appPath/Robot/appsettings.json" "$releasePath/docker部署Sers/sers-demo-robot"
cp -rf "$appPath/Robot/." "$releasePath/docker制作镜像Sers/sers-demo-robot/app"



echo "copy sers-demo-sersall"
cp -rf "$releasePath/Sers压测/sers压测-单体压测$netVersion/ServiceCenter/appsettings.json" "$releasePath/docker部署Sers/sers-demo-sersall"
cp -rf "$releasePath/Sers压测/sers压测-单体压测$netVersion/ServiceCenter" "$releasePath/docker制作镜像Sers/sers-demo-sersall/app"





#---------------------------------------------------------------------
#(x.3)docker-初始化多架构构建器

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
#(x.4)docker-构建多架构镜像（ arm、arm64 和 amd64 ）并推送到 Docker Hub

#docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD

for name in sers sers-gateway sers-gover sers-demo sers-demo-robot 
do
	echo "docker build $name"
	#docker buildx build $codePath/Publish/release/Sers/docker制作镜像Sers/$name -t $DOCKER_USERNAME/$name:$version -t $DOCKER_USERNAME/$name --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
done




 
