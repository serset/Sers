set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export appVersion=1.0

export DOCKER_ImagePrefix=serset/
export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx
export DOCKER_BuildxExtArgs=

# "




#---------------------------------------------------------------------
echo "75.docker-image-build-push_cross.bash -> #1 docker - init buildx"


export builderName="mybuilder__${appVersion}__"
echo "builderName: $builderName"


echo "#1.1 docker buildx version"
docker buildx version

echo "#1.2 install binfmt_misc"
docker run --privileged --rm tonistiigi/binfmt --install all

echo "#1.3 create builder"
if [ ! "$(docker buildx ls | grep $builderName)" ]; then docker buildx create --use --name $builderName --buildkitd-flags '--allow-insecure-entitlement security.insecure'; fi

echo "#1.4 start builder"
docker buildx inspect $builderName --bootstrap

echo "#1.5 show builders and supported CPU platform"
docker buildx ls



#---------------------------------------------------------------------
echo "75.docker-image-build-push_cross.bash -> #2 docker - build and push"

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
    echo "docker buildx build --allow security.insecure $dockerPath/$dockerName -t ${DOCKER_ImagePrefix}$dockerName:$appVersion -t ${DOCKER_ImagePrefix}$dockerName --platform=$platform --push $DOCKER_BuildxExtArgs --builder $builderName"
    docker buildx build --allow security.insecure $dockerPath/$dockerName -t ${DOCKER_ImagePrefix}$dockerName:$appVersion -t ${DOCKER_ImagePrefix}$dockerName --platform=$platform --push $DOCKER_BuildxExtArgs --builder $builderName
  fi
done


#---------------------------------------------------------------------
echo "75.docker-image-build-push_cross.bash -> #3 remove builder"
if [ "$(docker buildx ls | grep $builderName)" ]; then docker buildx rm $builderName; fi
