
cd  /root/app/Robot

while [ 1 -lt 2 ]
do
    echo "dotnet App.Robot.Station.dll"
    dotnet App.Robot.Station.dll
    sleep 4
done