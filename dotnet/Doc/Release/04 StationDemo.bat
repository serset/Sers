cd /d StationDemo

:begin
dotnet App.StationDemo.Station.dll

 
 TIMEOUT /T 10
@echo restart
goto begin