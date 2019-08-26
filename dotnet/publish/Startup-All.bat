@echo  off

@echo 01 ServiceCenter
cd /d ServiceCenter 
start /b dotnet App.ServiceCenter.dll > log.txt
cd ..

@echo 02 Gover
cd /d Gover
start /b dotnet App.Gover.Gateway.dll > log.txt
cd ..
 
@echo 03 Gateway
cd /d Gateway
start /b dotnet App.Gateway.dll > log.txt
cd ..
 

@echo 04 StationDemo
cd /d StationDemo
start /b dotnet App.StationDemo.Station.dll > log.txt
cd ..
 

@echo 05 Robot
cd /d Robot
start /b dotnet App.Robot.Station.dll > log.txt
cd ..
 
@echo Æô¶¯³É¹¦

start http://localhost:6022/index.html