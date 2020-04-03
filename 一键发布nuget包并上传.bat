cd /d "dotnet\Doc\Publish"
call "pack sers_lib.bat"
call "push sers_lib to SersNugetServer.bat"
cd /d ../../../
 
echo succeed
echo succeed
echo succeed

