cd /d Demo

:begin
dotnet App.Demo.Station.dll

 
 TIMEOUT /T 10
@echo restart
goto begin