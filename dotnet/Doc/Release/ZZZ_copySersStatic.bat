

@echo copy ReleaseFile
xcopy  "..\ReleaseFile\SersPublish" "SersPublish" /e /i /r /y


@echo copy ServiceCenter wwwroot
 xcopy  "..\..\netcore\ServiceCenter\App.Gover.Gateway\wwwroot" "SersPublish\ServiceCenter\wwwroot" /e /i /r /y

@echo copy Gover wwwroot
xcopy  "..\..\netcore\ServiceCenter\App.Gover.Gateway\wwwroot" "SersPublish\Gover\wwwroot" /e /i /r /y

@echo copy Robot wwwroot
xcopy  "..\..\netcore\Station\App.Robot.Station\wwwroot" "SersPublish\Robot\wwwroot" /e /i /r /y




@echo copy  xml of ServiceCenter
xcopy  "..\..\netcore\ServiceCenter\App.ServiceCenter\bin\Debug\netcoreapp2.1\*.xml" "SersPublish\ServiceCenter" /i /r /y
 
@echo copy  xml of Robot
xcopy  "..\..\netcore\Station\App.Robot.Station\bin\Debug\netcoreapp2.1\*.xml" "SersPublish\Robot" /i /r /y
 
@echo copy  xml of Demo
xcopy  "..\..\StationDemo\SersLoader\Did.SersLoader.Demo\bin\Debug\netcoreapp2.1\*.xml" "SersPublish\Demo" /i /r /y

@echo succeed
 