set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "


#---------------------------------------------------------------------
#41.extra-publish.sh
bashFile="$PWD/../../environment/build-bash__41.extra-publish.sh"
if [ -f "$bashFile" ]; then
	echo '#41.extra-publish.sh'
	sh "$bashFile"
fi



