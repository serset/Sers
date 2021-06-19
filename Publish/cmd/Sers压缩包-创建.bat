::获取当前版本号
:: set version=2.1.1.356 
for /f "tokens=3 delims=><" %%a in ('type ..\..\dotnet\Library\Sers\Sers.Core\Sers.Core\Sers.Core.csproj^|findstr "<Version>.*Version"') do set version=%%a



echo ["%version%"]

cd /d ..
 
mkdir release
mkdir release/Sers-%version%


echo 1.复制 nuget
xcopy "Publish/nuget" "release/Sers-%version%/nuget" /e /i /r /y 

echo 2.复制 SersPublish
xcopy "Publish/SersPublish" "release/Sers-%version%/SersPublish" /e /i /r /y

echo 3.复制 CL压测
xcopy "Publish/CL压测" "release/Sers-%version%/CL压测" /e /i /r /y

echo 4.复制 Sers压测
xcopy "Publish/Sers压测" "release/Sers-%version%/Sers压测" /e /i /r /y

echo 5.复制 SersDocker
xcopy "Publish/SersDocker/docker制作镜像Sers" "release/Sers-%version%/docker制作镜像Sers" /e /i /r /y

echo 6.复制 docker部署Sers
xcopy "Publish/SersDocker/docker部署Sers" "release/Sers-%version%/docker部署Sers" /e /i /r /y
 



echo 7.创建 Sers-%version%.zip
dotnet cmd/FileZip/FileZip.dll zip -p -i "release/Sers-%version%" -o "release/Sers-%version%.zip" 


echo %~n0.bat 执行成功！

cd /d cmd


:: pause

