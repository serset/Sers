@echo off


echo %~n0.bat start...


::(x.1)获取codePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ..\..\..
set codePath=%cd%
set publishPath=%codePath%\Publish\release\release\Station^(net6.0^)
set dockerPath=%codePath%\Publish\release\release\docker-deploy


rd /s /q "%dockerPath%"

::(x.2)copy dir
xcopy "%codePath%\Publish\ReleaseFile\docker-deploy" "%dockerPath%" /e /i /r /y



::(x.3)copy station 
xcopy "%publishPath%\ServiceCenter\appsettings.json" "%dockerPath%\sers"
xcopy "%publishPath%\Gateway\appsettings.json" "%dockerPath%\sers-gateway"
xcopy "%publishPath%\Gover\appsettings.json" "%dockerPath%\sers-gover"
xcopy "%publishPath%\Demo\appsettings.json" "%dockerPath%\sers-demo"
xcopy "%publishPath%\Robot\appsettings.json" "%dockerPath%\sers-demo-robot"
xcopy "%codePath%\Publish\release\release\压测\单体压测net6.0\ServiceCenter\appsettings.json" "%dockerPath%\sers-demo-sersall"
 




echo %~n0.bat 执行成功！

cd /d "%curPath%"