cd /d "dotnet\Doc\Publish"
call "nuget publish.bat"
call "nuget push to NugetServer.Sers.bat"
cd /d ../../../
 
echo succeed
echo succeed
echo succeed

