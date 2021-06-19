set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn

# "

 


echo ------------------------------------------------------------------
echo '(x.2)发布Sers压测CL'

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
set -e

echo 'publish Client'
cd /root/code/dotnet/Library/Sers/Sers.CL/Test/CommunicationManage/CmClient
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Publish/release/release/CL压测/CmClient

echo 'publish Server'
cd /root/code/dotnet/Library/Sers/Sers.CL/Test/CommunicationManage/CmServer
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Publish/release/release/CL压测/CmServer

echo 'copy bat'
\cp -rf /root/code/Publish/PublishFile/CL压测/. /root/code/Publish/release/release/CL压测

" 




echo ------------------------------------------------------------------
echo '(x.3)发布Sers压测'

for netVersion in netcoreapp2.1 net6.0
do

	appPath=${codePath}/Publish/release/release/SersPublish/${netVersion}

	#---------------------------------------------- 
	#单体压测
	echo "(x.4)sers压测-publish单体压测${netVersion}"
	targetPath=${codePath}/Publish/release/release/Sers压测/sers压测-单体压测${netVersion}
	mkdir -p $targetPath

	echo "(x.x.1)copy ServiceCenter"
	\cp -rf $appPath/ServiceCenter/. $targetPath/ServiceCenter

	echo "(x.x.2)copy demo"
	\cp -rf $appPath/Demo/wwwroot/. $targetPath/ServiceCenter/wwwroot
	\cp -rf $appPath/Demo/Did.SersLoader.Demo.dll $targetPath/ServiceCenter
	\cp -rf $appPath/Demo/Did.SersLoader.Demo.pdb $targetPath/ServiceCenter
	\cp -rf $appPath/Demo/Did.SersLoader.Demo.xml $targetPath/ServiceCenter 

	echo "(x.x.3)copy Robot"
	\cp -rf $appPath/Robot/wwwroot/. $targetPath/ServiceCenter/wwwroot
	\cp -rf $appPath/Robot/App.Robot.Station.dll $targetPath/ServiceCenter
	\cp -rf $appPath/Robot/App.Robot.Station.pdb $targetPath/ServiceCenter
	\cp -rf $appPath/Robot/App.Robot.Station.xml $targetPath/ServiceCenter 

	echo "(x.x.4)copy bat"
	\cp -rf $codePath/Publish/PublishFile/Sers压测/单体压测/. $targetPath



	#---------------------------------------------- 
	#分布式压测
	echo "(x.5)sers压测-publish分布式压测${netVersion}"
	targetPath=${codePath}/Publish/release/release/Sers压测/sers压测-分布式压测${netVersion}
	mkdir -p $targetPath

	echo "(x.x.1)copy  station"
	\cp -rf $appPath/ServiceCenter $targetPath
	\cp -rf $appPath/Demo $targetPath
	\cp -rf $appPath/Robot $targetPath

	echo "(x.x.2)copy bat"
	\cp -rf $codePath/Publish/PublishFile/Sers压测/分布式压测/. $targetPath


done





