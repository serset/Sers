cd /d ServiceCenter

:begin
dotnet App.ServiceCenter.dll

 TIMEOUT /T 1
@echo restart
goto begin
 