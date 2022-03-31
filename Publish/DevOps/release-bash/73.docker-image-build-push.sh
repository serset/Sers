set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

export version=`grep '<Version>' $(grep '<pack>\|<publish>' ${basePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

# "




#---------------------------------------------------------------------
#(x.2)docker-初始化多架构构建器

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
#(x.3)docker-构建多架构镜像（ arm、arm64 和 amd64 ）并推送到 Docker Hub

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD


dockerPath=$basePath/Publish/release/release/docker-image

for dockerName in `ls $dockerPath`
do
  if [ -d $dockerPath/$dockerName ]
  then 
    echo "docker build $dockerName"
    docker buildx build $dockerPath/$dockerName -t $DOCKER_USERNAME/$dockerName:$version -t $DOCKER_USERNAME/$dockerName --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
  fi
done



 
