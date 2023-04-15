set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "

if [ ! $NUGET_PATH ]; then NUGET_PATH=$basePath/Publish/release/.nuget; fi


#---------------------------------------------------------------------
#(x.2)
publishPath=$basePath/Publish/release/release/StressTest
mkdir -p $publishPath



echo ------------------------------------------------------------------
echo '(x.3)发布CL压测'

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $basePath:/root/code \
-v $NUGET_PATH:/root/.nuget \
serset/dotnet:sdk-6.0 \
bash -c "
set -e

echo 'publish Client'
cd /root/code/dotnet/Library/Sers/Sers.CL/Test/CommunicationManage/CmClient
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Publish/release/release/StressTest/CL压测net5.0/CmClient

echo 'publish Server'
cd /root/code/dotnet/Library/Sers/Sers.CL/Test/CommunicationManage/CmServer
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Publish/release/release/StressTest/CL压测net5.0/CmServer

echo 'copy bat'
\cp -rf /root/code/Publish/ReleaseFile/StressTest/CL压测/. /root/code/Publish/release/release/StressTest/CL压测net5.0

" 




echo ------------------------------------------------------------------
echo '(x.4)发布Sers压测'

for netVersion in netcoreapp2.1
do
	appPath=${basePath}/Publish/release/release/Station\(${netVersion}\)

	#---------------------------------------------- 
	#单体压测
	echo "(x.5)单体压测${netVersion}"
	targetPath=${publishPath}/单体压测${netVersion}
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

	echo "(x.x.4)copy ReleaseFile"
	\cp -rf $basePath/Publish/ReleaseFile/StressTest/单体压测/. $targetPath



	#---------------------------------------------- 
	#分布式压测
	echo "(x.6)分布式压测${netVersion}"
	targetPath=${publishPath}/分布式压测${netVersion}
	mkdir -p $targetPath

	echo "(x.x.1)copy  station"
	\cp -rf $appPath/ServiceCenter $targetPath
	\cp -rf $appPath/Demo $targetPath
	\cp -rf $appPath/Robot $targetPath

	echo "(x.x.2)copy ReleaseFile"
	\cp -rf $basePath/Publish/ReleaseFile/StressTest/分布式压测/. $targetPath

done





