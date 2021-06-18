:: start Sers\dotnet\Doc\Publish


call "nuget-pack.bat"
pause
call "sers-publish(netcoreapp2.1).bat"
pause
call "sers-publish(net6.0).bat"
pause
call "docker-build.bat"
pause
call "sers压测-publish.bat"
 pause

::call "Sers压缩包-创建.bat" 
 

echo %~n0.bat 执行成功！

pause