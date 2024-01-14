set -e


#---------------------------------------------------------------------
# args

args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "


#---------------------------------------------------------------------
echo '52.docker-extra-copy.sh -> #1 copy image files'

dockerPath="$basePath/Publish/release/release/docker-image"

echo "copy 单体压测"
\cp -rf "$basePath/Publish/release/release/StressTest/单体压测net6.0/ServiceCenter/." "$dockerPath/sers-demo-sersall/app"




#---------------------------------------------------------------------
echo '52.docker-extra-copy.sh -> #2 copy deploy files'


publishPath="$basePath/Publish/release/release/Station(net6.0)"
deployPath="$basePath/Publish/release/release/docker-deploy"
 
echo "copy station"

\cp -rf "$publishPath/ServiceCenter/appsettings.json" "$deployPath/sers"
\cp -rf "$publishPath/Gateway/appsettings.json" "$deployPath/sers-gateway"
\cp -rf "$publishPath/Gover/appsettings.json" "$deployPath/sers-gover"
\cp -rf "$publishPath/Demo/appsettings.json" "$deployPath/sers-demo"
\cp -rf "$publishPath/Robot/appsettings.json" "$deployPath/sers-demo-robot"
\cp -rf "$basePath/Publish/release/release/StressTest/单体压测net6.0/ServiceCenter/appsettings.json" "$deployPath/sers-demo-sersall"
