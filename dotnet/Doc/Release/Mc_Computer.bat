cd /d Mc_Computer

:begin
dotnet App.Mc_Computer.McStation.dll

 
TIMEOUT /T 60
@echo restart
goto begin
 

 
 