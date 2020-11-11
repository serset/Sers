

@echo "copy  SersDocker"
xcopy  "..\PublishFile\SersDocker" "SersDocker" /e /i /r /y

@echo "copy Demo"
xcopy  "SersPublish\Demo\appsettings.json" "SersDocker\docker部署Sers\Demo" 
xcopy  "SersPublish\Demo" "SersDocker\docker制作镜像Sers\sers_dotnet_demo\root\app\Demo" /e /i /r /y

@echo "copy Gateway"
xcopy  "SersPublish\Gateway\appsettings.json" "SersDocker\docker部署Sers\Gateway" 
xcopy  "SersPublish\Gateway" "SersDocker\docker制作镜像Sers\sers_dotnet_gateway\root\app\Gateway" /e /i /r /y

@echo "copy Gover"
xcopy  "SersPublish\Gover\appsettings.json" "SersDocker\docker部署Sers\Gover" 
xcopy  "SersPublish\Gover" "SersDocker\docker制作镜像Sers\sers_dotnet_gover\root\app\Gover" /e /i /r /y

@echo "copy Robot"
xcopy  "SersPublish\Robot\appsettings.json" "SersDocker\docker部署Sers\Robot" 
xcopy  "SersPublish\Robot" "SersDocker\docker制作镜像Sers\sers_dotnet_robot\root\app\Robot" /e /i /r /y

@echo "copy ServiceCenter"
xcopy  "SersPublish\ServiceCenter\appsettings.json" "SersDocker\docker部署Sers\ServiceCenter" 
xcopy  "SersPublish\ServiceCenter" "SersDocker\docker制作镜像Sers\sers_dotnet_servicecenter\root\app\ServiceCenter" /e /i /r /y

@echo "copy SersAll"
xcopy  "Sers单体压测\ServiceCenter\appsettings.json" "SersDocker\docker部署Sers\SersAll" 
xcopy  "Sers单体压测\ServiceCenter" "SersDocker\docker制作镜像Sers\sers_dotnet_sersall\root\app\ServiceCenter" /e /i /r /y
 