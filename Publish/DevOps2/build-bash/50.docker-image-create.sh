set -e


#---------------------------------------------------------------------
# 参数
args_="

export basePath=/root/temp/svn

# "

 
#---------------------------------------------------------------------
echo "#1 copy docker-image from ReleaseFile"

publishPath="$basePath/Publish/release/release/Station(net5.0)"
dockerPath=$basePath/Publish/release/release/docker-image


\cp -rf "$basePath/Publish/ReleaseFile/docker-image/." "$dockerPath"


#---------------------------------------------------------------------
echo "#2 copy station"
for file in $(find $basePath -name *.csproj -exec grep '<docker>' -l {} \;)
do
	cd $basePath
	
	publishName=`grep '<publish>' $file -r | grep -oE '\>(.*)\<' | tr -d '<>/'`
	
	dockerName=`grep '<docker>' $file -r | grep -oE '\>(.*)\<' | tr -d '<>/'`

	echo "#2.* copy $dockerName, publishName:$publishName"
	\cp -rf "$publishPath/$publishName/." "$dockerPath/$dockerName/app"
done


echo "#3 copy 单体压测"
\cp -rf "$basePath/Publish/release/release/StressTest/单体压测net5.0/ServiceCenter/." "$dockerPath/sers-demo-sersall/app"
 


