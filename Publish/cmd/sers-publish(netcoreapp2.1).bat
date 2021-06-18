@echo off

::启用变量延迟
setlocal EnableDelayedExpansion


::(x.1)初始化
set netVersion=netcoreapp2.1
echo publish sers
echo dotnet version: %netVersion%





::(x.2)获取basePath
set curPath=%cd%
cd /D %~dp0
cd /d ../..
set basePath=%cd%
set publishPath=%cd%\Publish\Publish\SersPublish\%netVersion%

::(x.2)查找所有需要发布nuget的项目并发布
for /f "delims=" %%f in ('findstr /M /s /i "<publish>" *.csproj') do (
	::get name
	for /f "tokens=3 delims=><" %%a in ('type %basePath%\%%f^|findstr "<publish>.*publish"') do set name=%%a
	echo publish !name!

	::publish
	cd /d %basePath%\%%f\.. 
	::dotnet build --configuration Release
	dotnet publish --configuration Release --output "%publishPath%\!name!"
	@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
	pause
	echo "copy xml"
	xcopy  "bin\Release\%netVersion%\*.xml" "%publishPath%\!name!" /i /r /y
	pause
)

cd /d %curPath%

echo 'pack nuget succeed！'
echo 'pack nuget succeed！'





:: 1  publish

cd /d ../../


 

 
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


echo 'publish sers succeed！'
echo 'publish sers succeed！'
echo 'publish sers succeed！'


