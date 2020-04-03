cd /d ../../

echo 'publish Client'
cd /d Sers\Sers.CL\Test\CommunicationManage\Client
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\Doc\Publish\CLÑ¹²â\CLClient
cd /d ../../../../../
   
echo 'publish Client'
cd /d Sers\Sers.CL\Test\CommunicationManage\Server
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\Doc\Publish\CLÑ¹²â\CLServer
cd /d ../../../../../

cd /d Doc\Publish

@echo "copy from PublishFile"
 xcopy  "..\PublishFile\CLÑ¹²â" "CLÑ¹²â" /e /i /r /y

echo 'publish CLÑ¹²â succeed£¡'
echo 'publish CLÑ¹²â succeed£¡'
echo 'publish CLÑ¹²â succeed£¡'


