@echo off 

cd /d nuget
 
for /R %%s in (*.nupkg) do ( 
echo push %%s 
dotnet nuget push %%s  -k oy2icd63utqxwo7fylpqdfrs46rt5mzehjsxy3ed7ca3je  -s https://api.nuget.org/v3/index.json
) 


cd /d ..

echo 'push sers_lib to Microsoft succeed£¡'
echo 'push sers_lib to Microsoft succeed£¡'
echo 'push sers_lib to Microsoft succeed£¡'


pause