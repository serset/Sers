cd /d Gateway

:begin
dotnet App.Gateway.dll

TIMEOUT /T 1
@echo restart
goto begin
 