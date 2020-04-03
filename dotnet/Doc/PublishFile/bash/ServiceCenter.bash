
cd   /root/app/ServiceCenter

while [ 1 -lt 2 ]
do
    echo "dotnet App.ServiceCenter.dll"
    dotnet App.ServiceCenter.dll
    sleep 1
done