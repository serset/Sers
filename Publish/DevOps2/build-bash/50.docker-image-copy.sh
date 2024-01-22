set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

# "

 
#---------------------------------------------------------------------
#1 copy docker image from ReleaseFile

publishPath="$basePath/Publish/release/release/Station(net6.0)"
dockerPath=$basePath/Publish/release/release/docker-image


if [ -d "$basePath/Publish/ReleaseFile/docker-image" ]; then
	echo "50.docker-image-copy.sh -> #1 copy docker image from ReleaseFile"
	\cp -rf "$basePath/Publish/ReleaseFile/docker-image/." "$dockerPath"
fi




#---------------------------------------------------------------------
echo "50.docker-image-copy.sh -> #2 copy station"
for file in $(find $basePath -name *.csproj -exec grep '<docker>' -l {} \;)
do
	cd $basePath
	
	publishName=`grep '<publish>' $file -r | grep -oE '\>(.*)\<' | tr -d '<>/'`
	
	dockerName=`grep '<docker>' $file -r | grep -oE '\>(.*)\<' | tr -d '<>/'`

	echo "#2.* copy $dockerName, publishName:$publishName"
	\cp -rf "$publishPath/$publishName/." "$dockerPath/$dockerName/app"
done






