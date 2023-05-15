set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

export appVersion=1.0

export DOCKER_SERVER=
export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

# "




#---------------------------------------------------------------------
echo "73.docker-image-build-push.sh -> #1 docker - init buildx"


export builderName="mybuilder__${appVersion}__"
echo "builderName: $builderName"

echo "#1.1 开启实验特性"
export DOCKER_CLI_EXPERIMENTAL=enabled

echo "#1.2 验证是否开启"
docker buildx version

echo "#1.3 启用binfmt_misc"
docker run --rm --privileged docker/binfmt:66f9012c56a8316f9244ffd7622d7c21c1f6f28d

echo "#1.4 验证binfmt_misc是否开启"
ls -al /proc/sys/fs/binfmt_misc/


echo "#1.5 创建构建器"
if [ ! "$(docker buildx ls | grep $builderName)" ]; then docker buildx create --use --name $builderName --buildkitd-flags '--allow-insecure-entitlement security.insecure'; fi

echo "#1.6 启动构建器"
docker buildx inspect $builderName --bootstrap

echo "#1.7 查看当前使用的构建器及构建器支持的CPU架构"
docker buildx ls



#---------------------------------------------------------------------
echo "73.docker-image-build-push.sh -> #2 docker - build and push"

echo "#2.1 login if UserName is not empty"
if [ -n "$DOCKER_USERNAME" ]; then docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD; fi

dockerPath=$basePath/Publish/release/release/docker-image

for dockerName in `ls $dockerPath`
do
  if [ -d $dockerPath/$dockerName ]
  then
    platform="linux/amd64,linux/arm64,linux/arm/v7"
    if [ -f "$dockerPath/$dockerName/Dockerfile.platform" ]; then platform=`cat "$dockerPath/$dockerName/Dockerfile.platform"`; fi

    echo "#2.* docker build $dockerName, platform: $platform"
    echo "docker buildx build $dockerPath/$dockerName -t $DOCKER_SERVER/$dockerName:$appVersion -t $DOCKER_SERVER/$dockerName --platform=$platform --push  --output=type=registry,registry.insecure=true --builder $builderName"
    docker buildx build $dockerPath/$dockerName -t $DOCKER_SERVER/$dockerName:$appVersion -t $DOCKER_SERVER/$dockerName --platform=$platform --push  --output=type=registry,registry.insecure=true --builder $builderName
  fi
done


#---------------------------------------------------------------------
echo "73.docker-image-build-push.sh -> #3 docker - remove buildx"
if [ "$(docker buildx ls | grep $builderName)" ]; then docker buildx rm $builderName; fi
