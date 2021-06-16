set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet

# "

 

#---------------------------------------------- 
echo "(x.2)sers压测-publish单体压测(netcoreapp2.1)"

netVersion=netcoreapp2.1
basePath=$codePath/Doc/Publish/Sers压测/sers压测-单体压测$netVersion
mkdir -p $basePath

echo "copy  ServiceCenter"
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/ServiceCenter $basePath

echo "copy demo"
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/wwwroot $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/Did.SersLoader.Demo.dll $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/Did.SersLoader.Demo.pdb $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/Did.SersLoader.Demo.xml $basePath/ServiceCenter 

echo 'copy robot'
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/wwwroot $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/App.Robot.Station.dll $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/App.Robot.Station.pdb $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/App.Robot.Station.xml $basePath/ServiceCenter 

echo 'copy PublishFile'
\cp -rf $codePath/Doc/PublishFile/Sers压测/单体压测/. $basePath

 

#---------------------------------------------- 
echo "(x.3)sers压测-publish单体压测(net6.0)"

netVersion=net6.0
basePath=$codePath/Doc/Publish/Sers压测/sers压测-单体压测$netVersion
mkdir -p $basePath

echo "copy  ServiceCenter"
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/ServiceCenter $basePath

echo "copy demo"
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/wwwroot $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/Did.SersLoader.Demo.dll $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/Did.SersLoader.Demo.pdb $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo/Did.SersLoader.Demo.xml $basePath/ServiceCenter 

echo 'copy robot'
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/wwwroot $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/App.Robot.Station.dll $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/App.Robot.Station.pdb $basePath/ServiceCenter
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot/App.Robot.Station.xml $basePath/ServiceCenter 

echo 'copy PublishFile'
\cp -rf $codePath/Doc/PublishFile/Sers压测/单体压测/. $basePath





#---------------------------------------------- 
echo "(x.4)sers压测-publish分布式压测(netcoreapp2.1)"

netVersion=netcoreapp2.1
basePath=$codePath/Doc/Publish/Sers压测/sers压测-分布式压测$netVersion
mkdir -p $basePath

echo "copy station"
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/ServiceCenter $basePath
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo $basePath
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot $basePath

echo "copy PublishFile"
\cp -rf $codePath/Doc/PublishFile/Sers压测/分布式压测/. $basePath





#---------------------------------------------- 
echo "(x.5)sers压测-publish分布式压测(net6.0)"

netVersion=net6.0
basePath=$codePath/Doc/Publish/Sers压测/sers压测-分布式压测$netVersion
mkdir -p $basePath

echo "copy station"
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/ServiceCenter $basePath
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Demo $basePath
\cp -rf $codePath/Doc/Publish/SersPublish/$netVersion/Robot $basePath

echo "copy PublishFile"
\cp -rf $codePath/Doc/PublishFile/Sers压测/分布式压测/. $basePath




 