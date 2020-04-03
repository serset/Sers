

@echo "copy  ServiceCenter"
xcopy  "SersPublish\ServiceCenter" "Sers单体压测\ServiceCenter" /e /i /r /y

@echo "copy demo"
xcopy  "SersPublish\Demo\wwwroot" "Sers单体压测\ServiceCenter\wwwroot" /e /i /r /y
xcopy  "SersPublish\Demo\Did.SersLoader.Demo.dll" "Sers单体压测\ServiceCenter" /i /r /y
xcopy  "SersPublish\Demo\Did.SersLoader.Demo.pdb" "Sers单体压测\ServiceCenter" /i /r /y
xcopy  "SersPublish\Demo\Did.SersLoader.Demo.xml" "Sers单体压测\ServiceCenter" /i /r /y


@echo "copy  xml of Robot"
xcopy  "SersPublish\Robot\wwwroot" "Sers单体压测\ServiceCenter\wwwroot" /e /i /r /y
xcopy  "SersPublish\Robot\App.Robot.Station.dll" "Sers单体压测\ServiceCenter" /i /r /y
xcopy  "SersPublish\Robot\App.Robot.Station.pdb" "Sers单体压测\ServiceCenter" /i /r /y
xcopy  "SersPublish\Robot\App.Robot.Station.xml" "Sers单体压测\ServiceCenter" /i /r /y


@echo "copy PublishFile"
xcopy  "..\PublishFile\Sers单体压测" "Sers单体压测" /e /i /r /y