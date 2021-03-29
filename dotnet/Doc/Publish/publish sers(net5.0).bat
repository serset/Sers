

set netVersion=net5.0

:: 调用工具 替换csproj
cd ..\..\..\..
VsTool.exe replace -r --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>netcoreapp2.1</TargetFramework>" --new "<TargetFramework>net5.0</TargetFramework>"
cd Sers\dotnet\Doc\Publish

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
@echo "copy PublishFile"
xcopy  "..\PublishFile\SersPublish" "SersPublish\%netVersion%" /e /i /r /y


::(x.2)ServiceCenter
@echo "copy ServiceCenter wwwroot"
xcopy  "..\..\ServiceCenter\App.Gover.Gateway\wwwroot" "SersPublish\%netVersion%\ServiceCenter\wwwroot" /e /i /r /y

@echo "copy  xml of ServiceCenter"
xcopy  "..\..\ServiceCenter\App.ServiceCenter\bin\Release\%netVersion%\*.xml" "SersPublish\%netVersion%\ServiceCenter" /i /r /y



::(x.3)Gover
@echo "copy Gover wwwroot"
xcopy  "..\..\ServiceCenter\App.Gover.Gateway\wwwroot" "SersPublish\%netVersion%\Gover\wwwroot" /e /i /r /y



::(x.4)Robot
@echo "copy Robot wwwroot"
xcopy  "..\..\ServiceStation\Demo\StressTest\App.Robot.Station\wwwroot" "SersPublish\%netVersion%\Robot\wwwroot" /e /i /r /y
 
@echo "copy  xml of Robot"
xcopy  "..\..\ServiceStation\Demo\StressTest\App.Robot.Station\bin\Release\%netVersion%\*.xml" "SersPublish\%netVersion%\Robot" /i /r /y


 
::(x.5)
@echo "copy  xml of Demo"
xcopy  "..\..\ServiceStation\Demo\SersLoader\Did.SersLoader.Demo\bin\Release\%netVersion%\*.xml" "SersPublish\%netVersion%\Demo" /i /r /y



:: 调用工具 替换csproj
cd ..\..\..\..
VsTool.exe replace -r --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>net5.0</TargetFramework>" --new "<TargetFramework>netcoreapp2.1</TargetFramework>"
cd Sers\dotnet\Doc\Publish

echo 'publish sers succeed！'
echo 'publish sers succeed！'
echo 'publish sers succeed！'


