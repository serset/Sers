
::(x.1)
@echo "copy PublishFile"
xcopy  "..\PublishFile\SersPublish" "SersPublish" /e /i /r /y


::(x.2)ServiceCenter
@echo "copy ServiceCenter wwwroot"
xcopy  "..\..\ServiceCenter\App.Gover.Gateway\wwwroot" "SersPublish\ServiceCenter\wwwroot" /e /i /r /y

@echo "copy  xml of ServiceCenter"
xcopy  "..\..\ServiceCenter\App.ServiceCenter\bin\Debug\netcoreapp2.1\*.xml" "SersPublish\ServiceCenter" /i /r /y



::(x.3)Gover
@echo "copy Gover wwwroot"
xcopy  "..\..\ServiceCenter\App.Gover.Gateway\wwwroot" "SersPublish\Gover\wwwroot" /e /i /r /y



::(x.4)Robot
@echo "copy Robot wwwroot"
xcopy  "..\..\ServiceStation\Demo\StressTest\App.Robot.Station\wwwroot" "SersPublish\Robot\wwwroot" /e /i /r /y
 
@echo "copy  xml of Robot"
xcopy  "..\..\ServiceStation\Demo\StressTest\App.Robot.Station\bin\Debug\netcoreapp2.1\*.xml" "SersPublish\Robot" /i /r /y


 
::(x.5)
@echo "copy  xml of Demo"
xcopy  "..\..\ServiceStation\Demo\SersLoader\Did.SersLoader.Demo\bin\Debug\netcoreapp2.1\*.xml" "SersPublish\Demo" /i /r /y



@echo succeed
 