@echo off

::启用变量延迟
setlocal EnableDelayedExpansion






::(x.1)初始化
set netVersion=net6.0
echo publish sers
echo dotnet version: %netVersion%





::(x.2)获取basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../..
set basePath=%cd%
set publishPath=%basePath%/Publish/release/release/SersPublish/%netVersion%


::修改要发布项目的netcore版本号为net6.0
Publish\cmd\VsTool.exe replace -r --path "%basePath%" --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>netcoreapp2.1</TargetFramework>" --new "<TargetFramework>net6.0</TargetFramework>"






::(x.3)查找所有需要发布的项目并发布
for /f "delims=" %%f in ('findstr /M /s /i "<publish>" *.csproj') do (
	::get name
	for /f "tokens=3 delims=><" %%a in ('type "%basePath%\%%f"^|findstr "<publish>.*publish"') do set name=%%a
	echo publish !name!

	::publish
	cd /d "%basePath%\%%f\.."
	dotnet build --configuration Release
	dotnet publish --configuration Release --output "%publishPath%\!name!"
	@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 

	::copy xml
	xcopy  "bin\Release\%netVersion%\*.xml" "%publishPath%\!name!" /i /r /y
)


 


 
::(x.4)copy bat
xcopy "%basePath%\Publish\PublishFile\SersPublish" "%publishPath%" /e /i /r /y

 



::还原项目的版本号
cd /d "%basePath%"
Publish\cmd\VsTool.exe replace -r --path "%basePath%" --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>net6.0</TargetFramework>" --new "<TargetFramework>netcoreapp2.1</TargetFramework>"





echo %~n0.bat 执行成功！

cd /d "%curPath%"