:: start Sers\dotnet\Doc\Publish

cd /d Sers
call "nuget 一键发布并上传.bat"
cd /d ..


cd /d Sers\dotnet\Doc\Publish

call "publish sers(netcoreapp2.1).bat"
call "publish sers压测-单体压测(netcoreapp2.1).bat"
call "publish sers压测-分布式压测(netcoreapp2.1).bat"

call "publish sers(net6.0).bat"
call "publish sers压测-单体压测(net6.0).bat"
call "publish sers压测-分布式压测(net6.0).bat"

call "pack SersDocker.bat"


call "publish CL压测.bat"


cd /d ../../../..

 
cd /d 生成压缩包
call "创建Sers压缩包.bat" 
start Publish
cd /d ..

echo succeed
echo succeed
echo succeed

pause