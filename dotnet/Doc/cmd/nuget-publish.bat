cd /d ../../Library
 
::------------------Library

cd /d Library


echo 'pack Vit.Core'
cd /d Vit\Vit.Core\Vit.Core
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\Doc\Publish\nuget 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../


 
echo 'pack Vit.Net.Http.FormFile'
cd /d Vit\Vit.Net.Http.FormFile
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\Doc\Publish\nuget 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../


echo 'pack Vit.WebHost'
cd /d Vit\Vit.WebHost
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\Doc\Publish\nuget 
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../











echo 'pack Sers.Hardware'
cd /d Sers\Sers.Hardware\Sers.Hardware
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../ 


echo 'pack Sers.Core'
cd /d Sers\Sers.Core\Sers.Core
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../  




echo 'pack Sers.CL.Ipc.NamedPipe'
cd /d Sers\Sers.CL\Ipc\Sers.CL.Ipc.NamedPipe
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../

echo 'pack Sers.CL.Socket.Iocp'
cd /d Sers\Sers.CL\Socket\Sers.CL.Socket.Iocp
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../

echo 'pack Sers.CL.Socket.ThreadWait'
cd /d Sers\Sers.CL\Socket\Sers.CL.Socket.ThreadWait
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../


echo 'pack Sers.CL.WebSocket'
cd /d Sers\Sers.CL\WebSocket\Sers.CL.WebSocket
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../


::echo 'pack Sers.CL.ClrZmq.ThreadWait'
::cd /d Sers\Sers.CL\Zmq\ThreadWait\Sers.CL.ClrZmq.ThreadWait
::dotnet build --configuration Release
::dotnet pack --configuration Release --output ..\..\..\..\..\..\Doc\Publish\nuget
::@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
::cd /d ../../../../../

::echo 'pack Sers.CL.Zmq.FullDuplex'
::cd /d Sers\Sers.CL\Zmq\FullDuplex\Sers.CL.Zmq.FullDuplex
::dotnet build --configuration Release
::dotnet pack --configuration Release --output ..\..\..\..\..\..\Doc\Publish\nuget
::@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
::cd /d ../../../../../


echo 'pack Sers.ServiceStation'
cd /d Sers\Sers.ServiceStation\Sers.ServiceStation
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../ 


echo 'pack Sers.Gateway'
cd /d Sers\Sers.Gateway\Sers.Gateway
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../  




cd ..





::------------------ServiceCenter
 
echo 'pack Sers.ServiceCenter'
cd /d ServiceCenter\Sers.ServiceCenter\Sers.ServiceCenter
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../



echo 'pack Sers.Gover'
cd /d ServiceCenter\Sers.ServiceCenter\Sers.Gover
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../








::------------------Sers.Serslot


echo 'pack Sers.Serslot'
cd /d Library\Sers\Sers.Serslot\Sers.Serslot
dotnet build --configuration Release
dotnet pack --configuration Release --output ..\..\..\..\Doc\Publish\nuget
@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
cd /d ../../../../  


 
 

cd /d Doc\cmd

echo 'pack sers_lib succeed！'
echo 'pack sers_lib succeed！'
echo 'pack sers_lib succeed！'