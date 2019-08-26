cd /d SersKit

:begin
dotnet App.SersKit.Station.dll

 
TIMEOUT /T 60
@echo restart
goto begin
 

 
 