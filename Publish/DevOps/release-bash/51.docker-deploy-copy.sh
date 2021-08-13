set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

# "

 
#---------------------------------------------------------------------
#(x.2)
publishPath="$basePath/Publish/release/release/Station(net5.0)"
dockerPath=$basePath/Publish/release/release/docker-deploy



#----------------------------------------------
echo "(x.3)copy dir"
\cp -rf "$basePath/Publish/ReleaseFile/docker-deploy/." "$dockerPath"



#---------------------------------------------------------------------
echo "(x.4)copy station"

\cp -rf "$publishPath/ServiceCenter/appsettings.json" "$dockerPath/sers"
\cp -rf "$publishPath/Gateway/appsettings.json" "$dockerPath/sers-gateway"
\cp -rf "$publishPath/Gover/appsettings.json" "$dockerPath/sers-gover"
\cp -rf "$publishPath/Demo/appsettings.json" "$dockerPath/sers-demo"
\cp -rf "$publishPath/Robot/appsettings.json" "$dockerPath/sers-demo-robot"
\cp -rf "$basePath/Publish/release/release/StressTest/单体压测net6.0/ServiceCenter/appsettings.json" "$dockerPath/sers-demo-sersall"
 



