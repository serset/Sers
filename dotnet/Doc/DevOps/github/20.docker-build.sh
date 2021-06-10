set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/docker/jenkins/workspace/sqler/svn 



export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`

export name=sqler
export projectPath=Sqler

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

# "

 


#---------------------------------------------------------------------
echo "(x.2)dotnet-构建并发布项目文件"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
cd '/root/code/$projectPath'
dotnet build --configuration Release
dotnet publish --configuration Release --output '/root/code/Publish/06.Docker/制作镜像/$name/app' " 





#---------------------------------------------------------------------
#(x.3.1)docker-初始化多架构构建器

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
#(x.3.2)docker-构建多架构镜像（ arm、arm64 和 amd64 ）并推送到 Docker Hub

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD

docker buildx build $codePath/Publish/06.Docker/制作镜像/$name -t $DOCKER_USERNAME/$name:$version -t $DOCKER_USERNAME/$name --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
 




 




 
