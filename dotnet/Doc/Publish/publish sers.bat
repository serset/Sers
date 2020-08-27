cd /d ../../

echo 'publish Gateway'
cd /d netcore\Gateway\App.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Publish\SersPublish\Gateway 
cd /d ../../../
 
echo 'publish Gover'
cd /d netcore\ServiceCenter\App.Gover.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Publish\SersPublish\Gover 
cd /d ../../../
 
echo 'publish ServiceCenter'
cd /d netcore\ServiceCenter\App.ServiceCenter
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Publish\SersPublish\ServiceCenter
cd /d ../../../ 

echo 'publish Robot'
cd /d netcore\Station\App.Robot.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Publish\SersPublish\Robot
cd /d ../../../


echo 'publish Demo'
cd /d StationDemo\SersLoader\Did.SersLoader.Demo
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Publish\SersPublish\Demo
cd /d ../../../

 
cd /d Doc\Publish


call "ZZZ_copySersStatic.bat"


echo 'publish sers succeed£¡'
echo 'publish sers succeed£¡'
echo 'publish sers succeed£¡'


