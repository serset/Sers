set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

# "

 
#---------------------------------------------------------------------
#(x.2)
publishPath="$basePath/Publish/release/release/Station(net6.0)"
deployPath=$basePath/Publish/release/release/docker-deploy



#----------------------------------------------
echo "(x.3)copy dir"
\cp -rf "$basePath/Publish/ReleaseFile/docker-deploy/." "$deployPath"



#---------------------------------------------------------------------
echo "(x.4)copy station"

\cp -rf "$publishPath/ServiceCenter/appsettings.json" "$deployPath/sers"
\cp -rf "$publishPath/Gateway/appsettings.json" "$deployPath/sers-gateway"
\cp -rf "$publishPath/Gover/appsettings.json" "$deployPath/sers-gover"
\cp -rf "$publishPath/Demo/appsettings.json" "$deployPath/sers-demo"
\cp -rf "$publishPath/Robot/appsettings.json" "$deployPath/sers-demo-robot"
\cp -rf "$basePath/Publish/release/release/StressTest/µ•ÃÂ—π≤‚net6.0/ServiceCenter/appsettings.json" "$deployPath/sers-demo-sersall"




