@echo off

::启用变量延迟
setlocal EnableDelayedExpansion


::(x.1)获取basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../../..
set basePath=%cd%


::(x.2)获取netVersion
set netVersion=net5.0
for /f "tokens=3 delims=><" %%a in ('type %basePath%\dotnet\ServiceCenter\App.ServiceCenter\App.ServiceCenter.csproj^|findstr "<TargetFramework>.*TargetFramework"') do set netVersion=%%a


set publishPath=%basePath%/Publish/release/release/Station(%netVersion%)
echo publish Station
echo dotnet version: %netVersion%



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



::(x.4)copy dir
xcopy "%basePath%\Publish\ReleaseFile\Station" "%publishPath%" /e /i /r /y



::(x.5)copy ServiceCenter
xcopy "%publishPath%\ServiceCenter" "%basePath%\Publish\release\release\ServiceCenter(%netVersion%)\ServiceCenter" /e /i /r /y
xcopy "%publishPath%\01.ServiceCenter.bat" "%basePath%\Publish\release\release\ServiceCenter(%netVersion%)"
xcopy "%publishPath%\01.Start-4580.bat" "%basePath%\Publish\release\release\ServiceCenter(%netVersion%)"




echo %~n0.bat 执行成功！

cd /d "%curPath%"