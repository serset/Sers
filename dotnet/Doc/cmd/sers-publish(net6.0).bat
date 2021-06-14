

set netVersion=net6.0

:: 调用工具 替换csproj

VsTool.exe replace -r --path "..\.." --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>netcoreapp2.1</TargetFramework>" --new "<TargetFramework>net6.0</TargetFramework>"


echo publish sers
echo dotnet version: %netVersion%

:: 1  publish

cd /d ../../

echo 'publish Gateway'
cd /d Gateway\App.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\%netVersion%\Gateway 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../
 
 
echo 'publish Gover'
cd /d ServiceCenter\App.Gover.Gateway
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\%netVersion%\Gover 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../
 
echo 'publish ServiceCenter'
cd /d ServiceCenter\App.ServiceCenter
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\Doc\Publish\SersPublish\%netVersion%\ServiceCenter
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../





echo 'publish Demo'
cd /d ServiceStation\Demo\SersLoader\Did.SersLoader.Demo
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Publish\SersPublish\%netVersion%\Demo
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../


echo 'publish Robot'
cd /d ServiceStation\Demo\StressTest\App.Robot.Station
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\Doc\Publish\SersPublish\%netVersion%\Robot
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../


 
cd /d Doc\Publish

 
:: 2 copy Sers Static

::(x.1)
@echo "copy bat"
xcopy  "..\PublishFile\SersPublish" "SersPublish\%netVersion%" /e /i /r /y


::(x.2)copy xml
@echo "copy ServiceCenter xml "
xcopy  "..\..\ServiceCenter\App.ServiceCenter\bin\Release\%netVersion%\*.xml" "SersPublish\%netVersion%\ServiceCenter" /i /r /y

@echo "copy Robot xml"
xcopy  "..\..\ServiceStation\Demo\StressTest\App.Robot.Station\bin\Release\%netVersion%\*.xml" "SersPublish\%netVersion%\Robot" /i /r /y

@echo "copy Demo xml"
xcopy  "..\..\ServiceStation\Demo\SersLoader\Did.SersLoader.Demo\bin\Release\%netVersion%\*.xml" "SersPublish\%netVersion%\Demo" /i /r /y


cd /d ..\cmd



:: 调用工具 替换csproj
VsTool.exe replace -r --path "..\.." --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>net6.0</TargetFramework>" --new "<TargetFramework>netcoreapp2.1</TargetFramework>"
cd Sers\dotnet\Doc\Publish




echo 'publish sers succeed！'
echo 'publish sers succeed！'
echo 'publish sers succeed！'


