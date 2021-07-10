@echo off


echo %~n0.bat start...


::(x.1)获取basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../../..
set basePath=%cd%

::(x.2)
set publishPath=%basePath%/Publish/release/release/Station(net6.0)
set dockerPath=%basePath%/Publish/release/release/docker-image
rd /s /q "%dockerPath%"


::(x.3)copy dir
xcopy "%basePath%/Publish/ReleaseFile/docker-image" "%dockerPath%" /e /i /r /y


::(x.4)copy station 
xcopy  "%publishPath%/ServiceCenter" "%dockerPath%/sers/app" /e /i /r /y
xcopy  "%publishPath%/Gateway" "%dockerPath%/sers-gateway/app" /e /i /r /y
xcopy  "%publishPath%/Gover" "%dockerPath%/sers-gover/app" /e /i /r /y
xcopy  "%publishPath%/Demo" "%dockerPath%/sers-demo/app" /e /i /r /y
xcopy  "%publishPath%/Robot" "%dockerPath%/sers-demo-robot/app" /e /i /r /y
xcopy  "%basePath%/Publish/release/release/压测/单体压测net6.0/ServiceCenter" "%dockerPath%/sers-demo-sersall/app" /e /i /r /y
 




echo %~n0.bat 执行成功！

cd /d "%curPath%"