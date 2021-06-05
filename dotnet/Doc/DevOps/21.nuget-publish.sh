set -e



codePath=/root/code/Sers/dotnet
# codePath=/home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code/Sers/dotnet

cd ${codePath}
for file in $(grep -a '<TargetFramework>netstandard2.0</TargetFramework>' . -rl --include *.csproj)
do
    if ! [[ $file =~ (Apm|Empty|Temp|Zmq|\-) ]]; then
        echo pack $file
	cd $codePath
        cd $(dirname "$file")
        dotnet build --configuration Release
	dotnet pack --configuration Release --output '/root/code/Sers/dotnet/Doc/Publish/nuget'
    fi
done

