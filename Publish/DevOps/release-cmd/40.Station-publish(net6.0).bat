@echo off


::(x.1)初始化
set netVersion=net6.0


::(x.2)修改要发布项目的netcore版本号
VsTool.exe replace -r --path "../../.." --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>netcoreapp2.1</TargetFramework>" --new "<TargetFramework>%netVersion%</TargetFramework>"





::(x.3)发布项目
call "40.Station-publish.bat"






::(x.4)还原项目的版本号
VsTool.exe replace -r --path "../../.." --file "App.Gateway.csproj|App.Gover.Gateway.csproj|App.ServiceCenter.csproj|Did.SersLoader.Demo.csproj|App.Robot.Station.csproj" --old "<TargetFramework>%netVersion%</TargetFramework>" --new "<TargetFramework>netcoreapp2.1</TargetFramework>"





echo %~n0.bat 执行成功！
cd /d "%curPath%"