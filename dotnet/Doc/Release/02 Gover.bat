start http://localhost:6022/index.html

cd /d Gover

:begin
dotnet App.Gover.Gateway.dll

TIMEOUT /T 1
@echo restart
goto begin
 
 