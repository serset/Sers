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
echo "(x.2)docker - init buildx"

echo "开启实验特性"
export DOCKER_CLI_EXPERIMENTAL=enabled

echo "验证是否开启"
docker buildx version

echo "启用binfmt_misc"
docker run --rm --privileged docker/binfmt:66f9012c56a8316f9244ffd7622d7c21c1f6f28d

echo "验证binfmt_misc是否开启"
ls -al /proc/sys/fs/binfmt_misc/


echo "创建构建器"
if [ ! "$(docker buildx ls | grep mybuilder)" ]; then docker buildx create --use --name mybuilder; fi

echo "启动构建器"
docker buildx inspect mybuilder --bootstrap

echo "查看当前使用的构建器及构建器支持的CPU架构"
docker buildx ls



#---------------------------------------------------------------------
echo "(x.3)docker - build and push"

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


#---------------------------------------------------------------------
echo "(x.4)docker - remove buildx"
if [ "$(docker buildx ls | grep mybuilder)" ]; then docker buildx rm mybuilder; fi
