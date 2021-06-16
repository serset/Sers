

dotnet build --configuration Release
dotnet publish --configuration Release --output bin\DeliveryClient 
 


 
cd /d ..\DeliveryServer
dotnet build --configuration Release
dotnet publish --configuration Release --output bin\DeliveryServer 
 


