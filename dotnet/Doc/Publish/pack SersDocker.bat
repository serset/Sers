
set netVersion=net5.0

@echo "copy  SersDocker"
xcopy  "..\PublishFile\SersDocker" "SersDocker" /e /i /r /y

@echo "copy Demo"
xcopy  "SersPublish\%netVersion%\Demo\appsettings.json" "SersDocker\docker部署Sers\Demo" 
xcopy  "SersPublish\%netVersion%\Demo" "SersDocker\docker制作镜像Sers\demo_station\root\app\Demo" /e /i /r /y

@echo "copy Gateway"
xcopy  "SersPublish\%netVersion%\Gateway\appsettings.json" "SersDocker\docker部署Sers\Gateway" 
xcopy  "SersPublish\%netVersion%\Gateway" "SersDocker\docker制作镜像Sers\gateway\root\app\Gateway" /e /i /r /y

@echo "copy Gover"
xcopy  "SersPublish\%netVersion%\Gover\appsettings.json" "SersDocker\docker部署Sers\Gover" 
xcopy  "SersPublish\%netVersion%\Gover" "SersDocker\docker制作镜像Sers\gover\root\app\Gover" /e /i /r /y

@echo "copy Robot"
xcopy  "SersPublish\%netVersion%\Robot\appsettings.json" "SersDocker\docker部署Sers\Robot" 
xcopy  "SersPublish\%netVersion%\Robot" "SersDocker\docker制作镜像Sers\demo_robot\root\app\Robot" /e /i /r /y

@echo "copy ServiceCenter"
xcopy  "SersPublish\%netVersion%\ServiceCenter\appsettings.json" "SersDocker\docker部署Sers\ServiceCenter" 
xcopy  "SersPublish\%netVersion%\ServiceCenter" "SersDocker\docker制作镜像Sers\servicecenter\root\app\ServiceCenter" /e /i /r /y

@echo "copy SersAll"
xcopy  "Sers单体压测\%netVersion%\ServiceCenter\appsettings.json" "SersDocker\docker部署Sers\SersAll" 
xcopy  "Sers单体压测\%netVersion%\ServiceCenter" "SersDocker\docker制作镜像Sers\demo_sersall\root\app\ServiceCenter" /e /i /r /y
 