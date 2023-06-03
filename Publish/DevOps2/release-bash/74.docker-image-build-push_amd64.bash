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
echo "74.docker-image-build-push_amd64.bash"

echo "#1 login if UserName is not empty"
if [ -n "$DOCKER_USERNAME" ]; then docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD; fi

dockerPath=$basePath/Publish/release/release/docker-image

for dockerName in `ls $dockerPath`
do
  if [ -d $dockerPath/$dockerName ]
  then
    echo "#2.* docker build $dockerName"
    echo "docker build $dockerPath/$dockerName -t ${DOCKER_ImagePrefix}$dockerName:$appVersion -t ${DOCKER_ImagePrefix}$dockerName"
    docker build $dockerPath/$dockerName -t ${DOCKER_ImagePrefix}$dockerName:$appVersion -t ${DOCKER_ImagePrefix}$dockerName
    docker push ${DOCKER_ImagePrefix}$dockerName:$appVersion
    docker push ${DOCKER_ImagePrefix}$dockerName
    docker rmi -f ${DOCKER_ImagePrefix}$dockerName:$appVersion ${DOCKER_ImagePrefix}$dockerName
  fi
done


