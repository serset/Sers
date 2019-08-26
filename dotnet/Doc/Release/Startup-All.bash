cd /root/netapp/ServiceCenter 
dotnet App.ServiceCenter.dll > log.txt &
 
sleep 1
 
cd /root/netapp/Gover
dotnet App.Gover.Gateway.dll > log.txt &
 
 
 
cd /root/netapp/Gateway
dotnet App.Gateway.dll > log.txt &
 
 


cd /root/netapp/StationDemo
dotnet App.StationDemo.Station.dll > log.txt &
 
 


cd /root/netapp/Robot
dotnet App.Robot.Station.dll > log.txt &
 