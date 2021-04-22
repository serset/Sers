
set netVersion=net6.0

@echo "copy  SersDocker"
xcopy  "..\PublishFile\SersDocker" "SersDocker" /e /i /r /y

@echo "copy sers"
xcopy  "SersPublish\%netVersion%\ServiceCenter\appsettings.json" "SersDocker\docker部署Sers\sers" 
xcopy  "SersPublish\%netVersion%\ServiceCenter" "SersDocker\docker制作镜像Sers\sers\root\app\ServiceCenter" /e /i /r /y


@echo "copy sers_demo"
xcopy  "SersPublish\%netVersion%\Demo\appsettings.json" "SersDocker\docker部署Sers\sers_demo" 
xcopy  "SersPublish\%netVersion%\Demo" "SersDocker\docker制作镜像Sers\sers_demo\root\app\Demo" /e /i /r /y

@echo "copy sers_demo_robot"
xcopy  "SersPublish\%netVersion%\Robot\appsettings.json" "SersDocker\docker部署Sers\sers_demo_robot" 
xcopy  "SersPublish\%netVersion%\Robot" "SersDocker\docker制作镜像Sers\sers_demo_robot\root\app\Robot" /e /i /r /y

@echo "copy sers_demo_sersall"
xcopy  "Sers单体压测\%netVersion%\ServiceCenter\appsettings.json" "SersDocker\docker部署Sers\sers_demo_sersall" 
xcopy  "Sers单体压测\%netVersion%\ServiceCenter" "SersDocker\docker制作镜像Sers\sers_demo_sersall\root\app\ServiceCenter" /e /i /r /y



@echo "copy sers_gateway"
xcopy  "SersPublish\%netVersion%\Gateway\appsettings.json" "SersDocker\docker部署Sers\sers_gateway" 
xcopy  "SersPublish\%netVersion%\Gateway" "SersDocker\docker制作镜像Sers\sers_gateway\root\app\Gateway" /e /i /r /y

@echo "copy sers_gover"
xcopy  "SersPublish\%netVersion%\Gover\appsettings.json" "SersDocker\docker部署Sers\sers_gover" 
xcopy  "SersPublish\%netVersion%\Gover" "SersDocker\docker制作镜像Sers\sers_gover\root\app\Gover" /e /i /r /y





 