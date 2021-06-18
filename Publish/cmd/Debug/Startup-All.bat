@echo  off
cd /d ..\..\..\..\..

@echo 01 ServiceCenter
cd /d App\ServiceCenter\App.ServiceCenter\bin\Debug\netcoreapp2.1
start /b dotnet App.ServiceCenter.dll > log.txt
cd ..\..\..\..\..\..
 

@echo 02 Gover
cd /d App\ServiceCenter\Gover\UI\App.Gover.Gateway\bin\Debug\netcoreapp2.1
start /b dotnet App.Gover.Gateway.dll > log.txt
cd ..\..\..\..\..\..\..\..
 
@echo 03 Gateway
cd /d App\Gateway\App.Gateway\bin\Debug\netcoreapp2.1
start /b dotnet App.Gateway.dll > log.txt
cd ..\..\..\..\..\..
 

@echo 04 Demo
cd /d App\Station\Demo\App.Demo.Station\bin\Debug\netcoreapp2.1
start /b dotnet App.Demo.Station.dll > log.txt
cd ..\..\..\..\..\..\..
 

@echo 05 Robot
cd /d App\Station\Robot\App.Robot.Station\bin\Debug\netcoreapp2.1
start /b dotnet App.Robot.Station.dll > log.txt
cd ..\..\..\..\..\..\..
 

@echo Æô¶¯³É¹¦

start http://localhost:6022/_gover_/index.html