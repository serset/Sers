set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export appVersion=1.0

export DOCKER_ImagePrefix=serset/
export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

export DOCKER_Buildx=false #default: true
export DOCKER_BuildxExtArgs=

# "




#---------------------------------------------------------------------
echo "73.docker-image-build-push.sh"

if [ ! -d "$basePath/Publish/release/release/docker-image" ]; then
    echo '73.docker-image-build-push.sh -> skip for no docker image files exist'
    exit 0
fi

if [ "$DOCKER_Buildx" != "false" ]; then
    sh 75.docker-image-build-push_cross.bash
else
    sh 74.docker-image-build-push_amd64.bash
fi



