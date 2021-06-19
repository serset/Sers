set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn

export version=`grep '<Version>' "${codePath}" -r --include Sers.Core.csproj | grep -oP '>(.*)<' | tr -d '<>'`


# "

 


#---------------------------------------------------------------------
echo "(x.2)copy SersDocker"

netVersion=net6.0
releasePath=$codePath/Publish/release/release
appPath=$codePath/Publish/release/release/SersPublish/$netVersion

echo "copy SersDocker"
cp -rf "$codePath/Publish/PublishFile/SersDocker/." "$releasePath"

echo "copy sers"
cp -rf "$appPath/ServiceCenter/appsettings.json" "$releasePath/docker部署Sers/sers"
cp -rf "$appPath/ServiceCenter/." "$releasePath/docker制作镜像Sers/sers/app"


echo "copy sers-gateway"
cp -rf "$appPath/Gateway/appsettings.json" "$releasePath/docker部署Sers/sers-gateway"
cp -rf "$appPath/Gateway/." "$releasePath/docker制作镜像Sers/sers-gateway/app"

echo "copy sers-gover"
cp -rf "$appPath/Gover/appsettings.json" "$releasePath/docker部署Sers/sers-gover"
cp -rf "$appPath/Gover/." "$releasePath/docker制作镜像Sers/sers-gover/app"



echo "copy sers-demo"
cp -rf "$appPath/Demo/appsettings.json" "$releasePath/docker部署Sers/sers-demo"
cp -rf "$appPath/Demo/." "$releasePath/docker制作镜像Sers/sers-demo/app" 

echo "copy sers-demo-robot"
cp -rf "$appPath/Robot/appsettings.json" "$releasePath/docker部署Sers/sers-demo-robot"
cp -rf "$appPath/Robot/." "$releasePath/docker制作镜像Sers/sers-demo-robot/app"



echo "copy sers-demo-sersall"
cp -rf "$releasePath/Sers压测/sers压测-单体压测$netVersion/ServiceCenter/appsettings.json" "$releasePath/docker部署Sers/sers-demo-sersall"
cp -rf "$releasePath/Sers压测/sers压测-单体压测$netVersion/ServiceCenter" "$releasePath/docker制作镜像Sers/sers-demo-sersall/app"


