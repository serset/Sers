cd /d ../../

echo 'publish Gateway'
cd /d Gateway\App.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\Gateway 
cd /d ../../


 
echo 'publish Gover'
cd /d ServiceCenter\App.Gover.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\Gover 
cd /d ../../
 
echo 'publish ServiceCenter'
cd /d ServiceCenter\App.ServiceCenter
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\ServiceCenter
cd /d ../../





echo 'publish Demo'
cd /d ServiceStation\Demo\SersLoader\Did.SersLoader.Demo
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Publish\SersPublish\Demo
cd /d ../../../../


echo 'publish Robot'
cd /d ServiceStation\Demo\StressTest\App.Robot.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Publish\SersPublish\Robot
cd /d ../../../../



 
cd /d Doc\Publish


call "ZZZ_copySersStatic.bat"


echo 'publish sers succeed£¡'
echo 'publish sers succeed£¡'
echo 'publish sers succeed£¡'


