::获取当前版本号
:: set version=2.1.1.356 
for /f "tokens=3 delims=><" %%a in ('type ..\..\Library\Vit\Vit.Core\Vit.Core\Vit.Core.csproj^|findstr "<Version>.*Version"') do set version=%%a



echo ["%version%"]

cd /d ..\Publish
 

mkdir Publish
mkdir Publish\Sers%version%



echo 1.创建 nuget-Sers.zip
dotnet ../cmd/FileZip/FileZip.dll zip -i "nuget" -o "Publish/Sers%version%/nuget-Sers%version%.zip"
 

echo 2.创建 SersPublish.zip
dotnet ../cmd/FileZip/FileZip.dll zip -i "SersPublish" -o "Publish/Sers%version%/SersPublish%version%.zip"

echo 3.创建 CL压测.zip
dotnet ../cmd/FileZip/FileZip.dll zip -i "CL压测" -o "Publish/Sers%version%/CL压测%version%.zip"

echo 4.创建 Sers压测.zip
dotnet ../cmd/FileZip/FileZip.dll zip -i "Sers压测" -o ".Publish/Sers%version%/Sers压测%version%.zip"




echo 5.创建 docker制作镜像Sers.zip
dotnet ../cmd/FileZip/FileZip.dll zip -i "SersDocker/docker制作镜像Sers" -o "Publish/Sers%version%/docker制作镜像Sers%version%.zip"

echo 6.创建 docker部署Sers.zip
dotnet ../cmd/FileZip/FileZip.dll zip -i "SersDocker/docker部署Sers" -o "Publish/Sers%version%/docker部署Sers%version%.zip" 


cd /d ..\cmd


:: pause

