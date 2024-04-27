set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "


#---------------------------------------------------------------------
#52.docker-extra-copy.sh
bashFile="$PWD/../environment/build-bash__52.docker-extra-copy.sh"
if [ -f "$bashFile" ]; then
	echo '#52.docker-extra-copy.sh'
	sh "$bashFile"
fi



