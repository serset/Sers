start http://localhost:6022/_gover_/index.html
 

:begin
dotnet Gover/App.Gover.Gateway.dll

TIMEOUT /T 2
@echo restart
goto begin
 
 