@echo off

::启用变量延迟
setlocal EnableDelayedExpansion



::(x.1)获取basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../../..
set basePath=%cd%



::(x.2)
set publishPath=%basePath%/Publish/release/release/StressTest





echo ------------------------------------------------------------------
echo "(x.3)发布CL压测"

::Client
cd /d "%basePath%\dotnet\Library\Sers\Sers.CL\Test\CommunicationManage\CmClient"
dotnet build --configuration Release
dotnet publish --configuration Release --output "%publishPath%\CL压测net6.0\CmClient"
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 

::Server
cd /d "%basePath%\dotnet\Library\Sers\Sers.CL\Test\CommunicationManage\CmServer"
dotnet build --configuration Release
dotnet publish --configuration Release --output "%publishPath%\CL压测net6.0\CmServer"
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 


::copy bat
xcopy  "%basePath%\Publish\ReleaseFile\StressTest\CL压测" "%publishPath%\CL压测net6.0" /e /i /r /y




echo ------------------------------------------------------------------
::(x.4)发布Sers压测
for %%i in (net6.0) do (  
	set netVersion=%%i
	set appPath=%basePath%/Publish/release/release/Station^(!netVersion!^)

	echo 发布 压测-!netVersion!

	::单体压测
	set targetPath=%publishPath%/单体压测!netVersion!

	::(x.x.1)copy ServiceCenter
	xcopy "!appPath!\ServiceCenter" "!targetPath!\ServiceCenter" /e /i /r /y

	::(x.x.2)copy demo
	xcopy "!appPath!\Demo\wwwroot" "!targetPath!\ServiceCenter\wwwroot" /e /i /r /y
	xcopy "!appPath!\Demo\Did.SersLoader.Demo.dll" "!targetPath!\ServiceCenter" /i /r /y
	xcopy "!appPath!\Demo\Did.SersLoader.Demo.pdb" "!targetPath!\ServiceCenter" /i /r /y
	xcopy "!appPath!\Demo\Did.SersLoader.Demo.xml" "!targetPath!\ServiceCenter" /i /r /y

	::(x.x.3)copy Robot
	xcopy "!appPath!\Robot\wwwroot" "!targetPath!\ServiceCenter\wwwroot" /e /i /r /y
	xcopy "!appPath!\Robot\App.Robot.Station.dll" "!targetPath!\ServiceCenter" /i /r /y
	xcopy "!appPath!\Robot\App.Robot.Station.pdb" "!targetPath!\ServiceCenter" /i /r /y
	xcopy "!appPath!\Robot\App.Robot.Station.xml" "!targetPath!\ServiceCenter" /i /r /y

	::(x.x.4)copy ReleaseFile
	xcopy "%basePath%\Publish\ReleaseFile\StressTest\单体压测" "!targetPath!" /e /i /r /y



	::分布式压测
	set targetPath=%publishPath%/分布式压测!netVersion!

	::(x.x.1)copy  station
	xcopy "!appPath!\ServiceCenter" "!targetPath!\ServiceCenter" /e /i /r /y
	xcopy "!appPath!\Demo" "!targetPath!\Demo" /e /i /r /y
	xcopy "!appPath!\Robot" "!targetPath!\Robot" /e /i /r /y

	::(x.x.2)copy ReleaseFile
	xcopy  "%basePath%\Publish\ReleaseFile\StressTest\分布式压测" "!targetPath!" /e /i /r /y
)



 


echo %~n0.bat 执行成功！

cd /d "%curPath%"



