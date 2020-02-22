start http://localhost:6022/_gover_/index.html

cd /d Gover

:begin
dotnet App.Gover.Gateway.dll

TIMEOUT /T 2
@echo restart
goto begin
 
 