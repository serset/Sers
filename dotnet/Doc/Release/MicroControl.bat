
cd /d MicroControl

:begin
dotnet App.MicroControl.Station.dll

TIMEOUT /T 2
@echo restart
goto begin
 

 
 