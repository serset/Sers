cd /d ../../

echo 'publish Gateway'
cd /d netcore\Gateway\App.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Release\Gateway 
cd /d ../../../
 
echo 'publish Gover'
cd /d netcore\ServiceCenter\App.Gover.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Release\Gover 
cd /d ../../../
 
echo 'publish ServiceCenter'
cd /d netcore\ServiceCenter\App.ServiceCenter
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\Doc\Release\ServiceCenter
cd /d ../../../

echo 'publish Robot'
cd /d netcore\Station\Robot\App.Robot.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Release\Robot
cd /d ../../../../

echo 'publish Demo'
cd /d netcore\Station\Demo\App.Demo.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Release\Demo
cd /d ../../../../

 
cd /d Doc\Release


call "ZZZ_copySersStatic.bat"


echo 'publish sers succeed£¡'
echo 'publish sers succeed£¡'
echo 'publish sers succeed£¡'


