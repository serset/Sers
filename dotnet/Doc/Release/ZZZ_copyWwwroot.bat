
@echo copy Gover wwwroot
xcopy  "..\..\netcore\ServiceCenter\App.Gover.Gateway\wwwroot" "Gover\wwwroot" /e /i /r /y



@echo copy Robot wwwroot
xcopy  "..\..\netcore\Station\Robot\App.Robot.Station\wwwroot" "Robot\wwwroot" /e /i /r /y




@echo copy Sers-CentosConfig
xcopy  "..\Linux\Sers-CentosConfig" "." /e /i /r /y

@echo copy  xml of ServiceCenter
xcopy  "..\..\netcore\ServiceCenter\App.ServiceCenter\bin\Debug\netcoreapp2.1\*.xml" "ServiceCenter" /i /r /y

@echo copy  xml of StationDemo
xcopy  "..\..\netcore\Station\StationDemo\App.StationDemo.Station\bin\Debug\netcoreapp2.1\*.xml" "StationDemo" /i /r /y
 
@echo copy  xml of Robot
xcopy  "..\..\netcore\Station\Robot\App.Robot.Station\bin\Debug\netcoreapp2.1\*.xml" "Robot" /i /r /y
 

@echo succeed
 