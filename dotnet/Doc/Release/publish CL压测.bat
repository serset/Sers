cd /d ../../

echo 'publish Client'
cd /d Sers\Sers.CL\Test\CommunicationManage\Client
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\Doc\Release\CLÑ¹²â\CLClient
cd /d ../../../../../
   
echo 'publish Client'
cd /d Sers\Sers.CL\Test\CommunicationManage\Server
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\Doc\Release\CLÑ¹²â\CLServer
cd /d ../../../../../

cd /d Doc\Release

@echo copy from ReleaseFile
 xcopy  "..\ReleaseFile\CLÑ¹²â" "CLÑ¹²â" /e /i /r /y

echo 'publish CLÑ¹²â succeed£¡'
echo 'publish CLÑ¹²â succeed£¡'
echo 'publish CLÑ¹²â succeed£¡'


pause