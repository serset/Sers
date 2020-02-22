cd /d "dotnet\Doc\Release"
call "pack sers_lib.bat"
call "push sers_lib to SersNugetServer.bat"

 
echo succeed
echo succeed
echo succeed

pause