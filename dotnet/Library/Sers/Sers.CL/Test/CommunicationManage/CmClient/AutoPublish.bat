 
dotnet build --configuration Release
dotnet publish --configuration Release --output bin\CmClient 
 


 
cd /d ..\CmServer
dotnet build --configuration Release
dotnet publish --configuration Release --output bin\CmServer
 


