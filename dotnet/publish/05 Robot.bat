cd /d Robot

:begin
dotnet App.Robot.Station.dll

 
 TIMEOUT /T 10
@echo restart
goto begin
 