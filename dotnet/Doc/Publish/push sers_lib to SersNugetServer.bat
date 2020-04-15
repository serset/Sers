@echo off 

cd /d nuget
 
for /R %%s in (*.nupkg) do ( 
echo push %%s 
dotnet nuget push %%s  -k ee28314c-f7fe-2550-bd77-e09eda3d0119  -s http://nuget.sers.cloud
) 


cd /d ..

echo 'push sers_lib to SersNugetServer succeed£¡'
echo 'push sers_lib to SersNugetServer succeed£¡'
echo 'push sers_lib to SersNugetServer succeed£¡'


