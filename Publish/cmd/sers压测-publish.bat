
cd /d ../../

echo ------------------------------------------------------------------
echo '(x.1.1)Sers—π≤‚CL-publish Client'
cd /d Library\Sers\Sers.CL\Test\CommunicationManage\CmClient
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\..\Doc\Publish\CL—π≤‚\CmClient
@if errorlevel 1 (echo . & echo .  & echo ≥ˆ¥Ì£¨«Î≈≈≤È£°& pause) 
cd /d ../../../../../../
   
echo '(x.1.2)Sers—π≤‚CL-publish Server'
cd /d Library\Sers\Sers.CL\Test\CommunicationManage\CmServer
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\..\Doc\Publish\CL—π≤‚\CmServer
@if errorlevel 1 (echo . & echo .  & echo ≥ˆ¥Ì£¨«Î≈≈≤È£°& pause) 
cd /d ../../../../../../

cd /d Doc\Publish

@echo "(x.1.3)Sers—π≤‚CL-copy bat"
 xcopy  "..\PublishFile\CL—π≤‚" "CL—π≤‚" /e /i /r /y


cd /d ../cmd




echo ------------------------------------------------------------------

cd /d ../Publish

set netVersion=netcoreapp2.1
set basePath=Sers—π≤‚\sers—π≤‚-µ•ÃÂ—π≤‚%netVersion%


@echo "(x.2.1)sers—π≤‚-publishµ•ÃÂ—π≤‚(netcoreapp2.1)-copy  ServiceCenter"
xcopy  "SersPublish\%netVersion%\ServiceCenter" "%basePath%\ServiceCenter" /e /i /r /y

@echo "copy demo"
xcopy  "SersPublish\%netVersion%\Demo\wwwroot" "%basePath%\ServiceCenter\wwwroot" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Demo\Did.SersLoader.Demo.dll" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Demo\Did.SersLoader.Demo.pdb" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Demo\Did.SersLoader.Demo.xml" "%basePath%\ServiceCenter" /i /r /y


@echo "(x.2.2)sers—π≤‚-publishµ•ÃÂ—π≤‚(netcoreapp2.1)-copy Robot"
xcopy  "SersPublish\%netVersion%\Robot\wwwroot" "%basePath%\ServiceCenter\wwwroot" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Robot\App.Robot.Station.dll" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Robot\App.Robot.Station.pdb" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Robot\App.Robot.Station.xml" "%basePath%\ServiceCenter" /i /r /y


@echo "(x.2.3)sers—π≤‚-publishµ•ÃÂ—π≤‚(netcoreapp2.1)-copy PublishFile"
xcopy  "..\PublishFile\Sers—π≤‚\µ•ÃÂ—π≤‚" "%basePath%" /e /i /r /y

cd /d ../cmd



echo ------------------------------------------------------------------


cd /d ../Publish


set netVersion=netcoreapp2.1
set basePath=Sers—π≤‚\sers—π≤‚-∑÷≤º Ω—π≤‚%netVersion%


@echo "(x.3.1)sers—π≤‚-publish∑÷≤º Ω—π≤‚(netcoreapp2.1)-copy  station"
xcopy  "SersPublish\%netVersion%\ServiceCenter" "%basePath%\ServiceCenter" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Demo" "%basePath%\Demo" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Robot" "%basePath%\Robot" /e /i /r /y


@echo "(x.3.2)sers—π≤‚-publish∑÷≤º Ω—π≤‚(netcoreapp2.1)-copy PublishFile"
xcopy  "..\PublishFile\Sers—π≤‚\∑÷≤º Ω—π≤‚" "%basePath%" /e /i /r /y

cd /d ../cmd


echo ------------------------------------------------------------------


cd /d ../Publish

set netVersion=net6.0
set basePath=Sers—π≤‚\sers—π≤‚-µ•ÃÂ—π≤‚%netVersion%


@echo "(x.4.1)sers—π≤‚-publishµ•ÃÂ—π≤‚(net6.0)-copy  ServiceCenter"
xcopy  "SersPublish\%netVersion%\ServiceCenter" "%basePath%\ServiceCenter" /e /i /r /y

@echo "(x.4.2)sers—π≤‚-publishµ•ÃÂ—π≤‚(net6.0)-copy demo"
xcopy  "SersPublish\%netVersion%\Demo\wwwroot" "%basePath%\ServiceCenter\wwwroot" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Demo\Did.SersLoader.Demo.dll" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Demo\Did.SersLoader.Demo.pdb" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Demo\Did.SersLoader.Demo.xml" "%basePath%\ServiceCenter" /i /r /y


@echo "(x.4.3)sers—π≤‚-publishµ•ÃÂ—π≤‚(net6.0)-copy Robot"
xcopy  "SersPublish\%netVersion%\Robot\wwwroot" "%basePath%\ServiceCenter\wwwroot" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Robot\App.Robot.Station.dll" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Robot\App.Robot.Station.pdb" "%basePath%\ServiceCenter" /i /r /y
xcopy  "SersPublish\%netVersion%\Robot\App.Robot.Station.xml" "%basePath%\ServiceCenter" /i /r /y


@echo "(x.4.4)sers—π≤‚-publishµ•ÃÂ—π≤‚(net6.0)-copy PublishFile"
xcopy  "..\PublishFile\Sers—π≤‚\µ•ÃÂ—π≤‚" "%basePath%" /e /i /r /y


cd /d ../cmd

echo ------------------------------------------------------------------



cd /d ../Publish


set netVersion=net6.0
set basePath=Sers—π≤‚\sers—π≤‚-∑÷≤º Ω—π≤‚%netVersion%


@echo "(x.5.1)sers—π≤‚-publish∑÷≤º Ω—π≤‚(net6.0)-copy  station"
xcopy  "SersPublish\%netVersion%\ServiceCenter" "%basePath%\ServiceCenter" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Demo" "%basePath%\Demo" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Robot" "%basePath%\Robot" /e /i /r /y


@echo "(x.5.2)sers—π≤‚-publish∑÷≤º Ω—π≤‚(net6.0)-copy PublishFile"
xcopy  "..\PublishFile\Sers—π≤‚\∑÷≤º Ω—π≤‚" "%basePath%" /e /i /r /y

cd /d ../cmd













