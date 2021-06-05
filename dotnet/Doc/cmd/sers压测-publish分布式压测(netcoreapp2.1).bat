
cd /d ../Publish


set netVersion=netcoreapp2.1
set basePath=Sers—π≤‚\sers—π≤‚-∑÷≤º Ω—π≤‚%netVersion%


@echo "copy  station"
xcopy  "SersPublish\%netVersion%\ServiceCenter" "%basePath%\ServiceCenter" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Demo" "%basePath%\Demo" /e /i /r /y
xcopy  "SersPublish\%netVersion%\Robot" "%basePath%\Robot" /e /i /r /y


@echo "copy PublishFile"
xcopy  "..\PublishFile\Sers—π≤‚\∑÷≤º Ω—π≤‚" "%basePath%" /e /i /r /y

cd /d ../cmd
