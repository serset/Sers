
call "nuget-pack.bat"

call "nuget-push to NugetServer.Sers.bat"

 
echo %~n0.bat 执行成功！

pause
