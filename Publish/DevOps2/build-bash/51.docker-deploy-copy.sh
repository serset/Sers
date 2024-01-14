set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

# "

 
#---------------------------------------------------------------------
#2
publishPath="$basePath/Publish/release/release/Station(net6.0)"
deployPath="$basePath/Publish/release/release/docker-deploy"



#----------------------------------------------
#3 copy dir
if [ -d "$basePath/Publish/ReleaseFile/docker-deploy" ]; then
	echo "51.docker-deploy-copy.sh -> copy dir"
	\cp -rf "$basePath/Publish/ReleaseFile/docker-deploy/." "$deployPath"
fi


