
call "nuget publish.bat"
cd /d "../Publish"
call "nuget-push to NugetServer.Sers"
cd /d ../cmd
 
echo succeed
echo succeed
echo succeed

