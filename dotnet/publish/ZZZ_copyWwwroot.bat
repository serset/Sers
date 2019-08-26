
@echo copy Gover wwwroot
xcopy  "..\App\ServiceCenter\Gover\UI\App.Gover.Gateway\wwwroot" "Gover\wwwroot" /e /i /r /y



@echo copy Robot wwwroot
xcopy  "..\App\Station\Robot\App.Robot.Station\wwwroot" "Robot\wwwroot" /e /i /r /y




@echo copy Sers-CentosConfig
xcopy  "..\Doc\Sers.Doc\Publish\Linux\Sers-CentosConfig" "." /e /i /r /y

@echo copy  xml of ServiceCenter
xcopy  "..\App\ServiceCenter\App.ServiceCenter\bin\Debug\netcoreapp2.1\*.xml" "ServiceCenter" /i /r /y

@echo copy  xml of StationDemo
xcopy  "..\App\Station\StationDemo\App.StationDemo.Station\bin\Debug\netcoreapp2.1\*.xml" "StationDemo" /i /r /y
 
@echo copy  xml of Robot
xcopy  "..\App\Station\Robot\App.Robot.Station\bin\Debug\netcoreapp2.1\*.xml" "Robot" /i /r /y
 
@echo copy  xml of AuthCenter
xcopy  "..\App\Station\AuthCenter\App.Station.AuthCenter\bin\Debug\netcoreapp2.1\*.xml" "AuthCenter" /i /r /y
 

@echo succeed
 