


call "nuget-pack.bat"

call "sers-publish(netcoreapp2.1).bat"

call "sers-publish(net6.0).bat"

call "sers压测-publish.bat"

call "docker-build.bat"

call "Sers压缩包-创建.bat" 
 

:: start ..\release


echo %~n0.bat 执行成功！

pause