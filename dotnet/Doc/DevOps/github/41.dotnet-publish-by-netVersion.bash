set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet
export netVersion=netcoreapp2.1 
# "

 

#----------------------------------------------
echo "(x.2)dotnet-publish"
echo "dotnet version: ${netVersion}"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
set -e

echo 'publish Gateway'
cd /root/code/Gateway/App.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/SersPublish/${netVersion}/Gateway

echo 'publish Gover'
cd /root/code/ServiceCenter/App.Gover.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/SersPublish/${netVersion}/Gover

echo 'publish ServiceCenter'
cd /root/code/ServiceCenter/App.ServiceCenter
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/SersPublish/${netVersion}/ServiceCenter

echo 'publish Demo'
cd /root/code/ServiceStation/Demo/SersLoader/Did.SersLoader.Demo
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/SersPublish/${netVersion}/Demo

echo 'publish Robot'
cd /root/code/ServiceStation/Demo/StressTest/App.Robot.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/SersPublish/${netVersion}/Robot
     
" 


#----------------------------------------------
echo "(x.3)dotnet-copy file"

echo "copy bat"
cp -rf "$codePath/Doc/PublishFile/SersPublish/." "$codePath/Doc/Publish/SersPublish/$netVersion"


echo "copy xml"
targetFolder="$codePath/Doc/Publish/SersPublish/$netVersion"

for file in "$codePath/ServiceCenter/App.ServiceCenter/bin/Release/$netVersion/*.xml" ; do cp -rf $file $targetFolder/ServiceCenter;done

for file in "$codePath/ServiceStation/Demo/StressTest/App.Robot.Station/bin/Release/$netVersion/*.xml" ; do cp -rf $file $targetFolder/Robot;done

for file in "$codePath/ServiceStation/Demo/SersLoader/Did.SersLoader.Demo/bin/Release/$netVersion/*.xml" ; do cp -rf $file $targetFolder/Demo;done








echo 'publish succeed！'






