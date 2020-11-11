
cd   /root/app/Gateway

while [ 1 -lt 2 ]
do
    echo "dotnet App.Gateway.dll"
    dotnet App.Gateway.dll
    sleep 4
done