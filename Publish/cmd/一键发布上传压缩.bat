:: start Sers\dotnet\Doc\Publish


call "nuget-一键发布并上传.bat"

call "sers-publish(netcoreapp2.1).bat"

call "sers-publish(net6.0).bat"

call "docker-build.bat"

call "sers压测-publish.bat"
 

call "Sers压缩包-创建.bat" 
 

echo succeed
echo succeed
echo succeed

pause