@echo off

::启用变量延迟
setlocal EnableDelayedExpansion


 




::(x.1)获取basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../..
set basePath=%cd%
set publishPath=%basePath%/Publish/release/release/CL压测





echo ------------------------------------------------------------------
:: 发布Sers压测CL
echo 发布Sers压测CL
::publish Client
cd /d "%basePath%\dotnet\Library\Sers\Sers.CL\Test\CommunicationManage\CmClient"
dotnet build --configuration Release
dotnet publish --configuration Release --output "%publishPath%\CmClient"
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 

::publish Server
cd /d "%basePath%\dotnet\Library\Sers\Sers.CL\Test\CommunicationManage\CmServer"
dotnet build --configuration Release
dotnet publish --configuration Release --output "%publishPath%\CmServer"
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 


::copy bat
xcopy  "%basePath%\Publish\PublishFile\CL压测" "%publishPath%" /e /i /r /y




echo ------------------------------------------------------------------
:: 发布Sers压测
for %%i in (netcoreapp2.1,net6.0) do (  
	set netVersion=%%i
	set appPath=%basePath%/Publish/release/release/SersPublish/!netVersion!

	echo 发布Sers压测-!netVersion!

	::单体压测
	set targetPath=%basePath%/Publish/release/release/Sers压测/sers压测-单体压测!netVersion!

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

	::(x.x.4)copy bat
	xcopy "%basePath%\Publish\PublishFile\Sers压测\单体压测" "!targetPath!" /e /i /r /y



	::分布式压测
	set targetPath=%basePath%/Publish/release/release/Sers压测/sers压测-分布式压测!netVersion!

	::(x.x.1)copy  station
	xcopy "!appPath!\ServiceCenter" "!targetPath!\ServiceCenter" /e /i /r /y
	xcopy "!appPath!\Demo" "!targetPath!\Demo" /e /i /r /y
	xcopy "!appPath!\Robot" "!targetPath!\Robot" /e /i /r /y

	::(x.x.2)copy bat
	xcopy  "%basePath%\Publish\PublishFile\Sers压测\分布式压测" "!targetPath!" /e /i /r /y
)




 

 


echo %~n0.bat 执行成功！

cd /d "%curPath%"









