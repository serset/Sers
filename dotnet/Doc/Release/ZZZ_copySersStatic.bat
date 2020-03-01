

@echo copy Sers-CentosConfig
xcopy  "..\Linux\Sers-CentosConfig" "." /e /i /r /y


@echo copy ServiceCenter wwwroot
 xcopy  "..\..\netcore\ServiceCenter\App.Gover.Gateway\wwwroot" "ServiceCenter\wwwroot" /e /i /r /y

@echo copy Gover wwwroot
xcopy  "..\..\netcore\ServiceCenter\App.Gover.Gateway\wwwroot" "Gover\wwwroot" /e /i /r /y

@echo copy Robot wwwroot
xcopy  "..\..\netcore\Station\App.Robot.Station\wwwroot" "Robot\wwwroot" /e /i /r /y




@echo copy  xml of ServiceCenter
xcopy  "..\..\netcore\ServiceCenter\App.ServiceCenter\bin\Debug\netcoreapp2.1\*.xml" "ServiceCenter" /i /r /y
 
@echo copy  xml of Robot
xcopy  "..\..\netcore\Station\App.Robot.Station\bin\Debug\netcoreapp2.1\*.xml" "Robot" /i /r /y
 
@echo copy  xml of Demo
xcopy  "..\..\StationDemo\SersLoader\Did.SersLoader.Demo\bin\Debug\netcoreapp2.1\*.xml" "Demo" /i /r /y

@echo succeed
 