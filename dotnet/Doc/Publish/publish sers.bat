cd /d ../../

echo 'publish Gateway'
cd /d Gateway\App.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\Gateway 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../
 
 
echo 'publish Gover'
cd /d ServiceCenter\App.Gover.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\Gover 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../
 
echo 'publish ServiceCenter'
cd /d ServiceCenter\App.ServiceCenter
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\ServiceCenter
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../





echo 'publish Demo'
cd /d ServiceStation\Demo\SersLoader\Did.SersLoader.Demo
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Publish\SersPublish\Demo
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../


echo 'publish Robot'
cd /d ServiceStation\Demo\StressTest\App.Robot.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Publish\SersPublish\Robot
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../



 
cd /d Doc\Publish


call "ZZZ_copySersStatic.bat"


echo 'publish sers succeed！'
echo 'publish sers succeed！'
echo 'publish sers succeed！'


