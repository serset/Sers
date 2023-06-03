set -e


#---------------------------------------------------------------------
#(x.1)参数
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

if [ "$DOCKER_Buildx" != "false" ]; then
	source 75.docker-image-build-push_cross.bash
else
	source 74.docker-image-build-push_amd64.bash
fi


