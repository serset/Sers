@echo off


:: #1 init
set netVersion=net5.0


:: #2 change netcore version
VsTool.exe replace -r --path "../../.." --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>net6.0</TargetFramework>" --new "<TargetFramework>%netVersion%</TargetFramework>"





:: #3 publish
call "40.Station-publish.bat"






:: #4 revert netcore version
VsTool.exe replace -r --path "../../.." --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>%netVersion%</TargetFramework>" --new "<TargetFramework>net6.0</TargetFramework>"





echo %~n0.bat success
cd /d "%curPath%"