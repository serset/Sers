@echo off 

cd /d ../release/release/nuget
 
for /R %%s in (*.nupkg) do ( 
echo push %%s 
dotnet nuget push "%%s"  -k ee28314c-f7fe-2550-bd77-e09eda3d0119  -s http://nuget.sers.cloud:8
) 


cd /d ../../../cmd

echo %~n0.bat 执行成功！


:: dotnet nuget delete ServiceAdaptor.NetCore.Sers 1.0.4.75 -k ee28314c-f7fe-2550-bd77-e09eda3d0119  -s http://nuget.sers.cloud --non-interactive