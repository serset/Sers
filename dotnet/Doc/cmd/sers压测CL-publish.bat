
cd /d ../../

echo 'publish Client'
cd /d Library\Sers\Sers.CL\Test\CommunicationManage\CmClient
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\..\Doc\Publish\CLÑ¹²â\CmClient
@if errorlevel 1 (echo . & echo .  & echo ³ö´í£¬ÇëÅÅ²é£¡& pause) 
cd /d ../../../../../../
   
echo 'publish Server'
cd /d Library\Sers\Sers.CL\Test\CommunicationManage\CmServer
dotnet build --configuration Release
dotnet publish --configuration Release --output ..\..\..\..\..\..\Doc\Publish\CLÑ¹²â\CmServer
@if errorlevel 1 (echo . & echo .  & echo ³ö´í£¬ÇëÅÅ²é£¡& pause) 
cd /d ../../../../../../

cd /d Doc\Publish

@echo "copy from PublishFile"
 xcopy  "..\PublishFile\CLÑ¹²â" "CLÑ¹²â" /e /i /r /y

echo 'publish CLÑ¹²â succeed£¡'
echo 'publish CLÑ¹²â succeed£¡'
echo 'publish CLÑ¹²â succeed£¡'


cd /d ../cmd