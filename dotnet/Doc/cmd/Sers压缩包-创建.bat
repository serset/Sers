::获取当前版本号
:: set version=2.1.1.356 
for /f "tokens=3 delims=><" %%a in ('type ..\..\Library\Sers\Sers.Core\Sers.Core\Sers.Core.csproj^|findstr "<Version>.*Version"') do set version=%%a



echo ["%version%"]

cd /d ..\Publish
 
mkdir release
mkdir release/Sers-%version%


echo 1.创建 nuget
xcopy "nuget" "release/Sers-%version%/nuget" /e /i /r /y 

echo 2.创建 SersPublish
xcopy "SersPublish" "release/Sers-%version%/SersPublish" /e /i /r /y

echo 3.创建 CL压测
xcopy "CL压测" "release/Sers-%version%/CL压测" /e /i /r /y

echo 4.创建 Sers压测
xcopy "Sers压测" "release/Sers-%version%/Sers压测" /e /i /r /y

echo 5.创建 docker制作镜像Sers
xcopy "SersDocker/docker制作镜像Sers" "release/Sers-%version%/docker制作镜像Sers" /e /i /r /y

echo 6.创建 docker部署Sers
xcopy "SersDocker/docker部署Sers" "release/Sers-%version%/docker部署Sers" /e /i /r /y
 



echo 7.创建 Sers-%version%.zip
dotnet ../cmd/FileZip/FileZip.dll zip -p -i "release/Sers-%version%" -o "release/Sers-%version%.zip" 


cd /d ..\cmd


:: pause

