set -e

cd '/root/code/Sers/dotnet/Library/Vit/Vit.Core/Vit.Core';
dotnet build --configuration Release; 
dotnet pack --configuration Release --output '/root/code/Sers/dotnet/Doc/Publish/nuget';


