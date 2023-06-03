set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "

if [ ! $NUGET_PATH ]; then NUGET_PATH=$basePath/Publish/release/.nuget; fi


#----------------------------------------------
echo "40.Station-publish.sh find projects and build"




docker run -i --rm \
--env LANG=C.UTF-8 \
-v $NUGET_PATH:/root/.nuget \
-v $basePath:/root/code \
serset/dotnet:sdk-6.0 \
bash -c "
set -e

echo '#1 get netVersion'
netVersion=\$(grep '<TargetFramework>' \$(grep '<publish>' -rl --include *.csproj /root/code | head -n 1) | grep -oP '>(.*)<' | tr -d '<>')
echo netVersion: \$netVersion


basePath=/root/code
publishPath=\$basePath/Publish/release/release/Station\(\$netVersion\)
mkdir -p \$publishPath

echo '#2 publish Station'
cd \$basePath
for file in \$(grep -a '<publish>' . -rl --include *.csproj)
do
	cd \$basePath
	
	#get publishName
	publishName=\`grep '<publish>' \$file -r | grep -oP '>(.*)<' | tr -d '<>'\`

	echo publish \$publishName

	#publish
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet publish --configuration Release --output \"\$publishPath/\$publishName\"

	#copy xml
	for filePath in bin/Release/\$netVersion/*.xml ; do \\cp -rf \$filePath \"\$publishPath/\$publishName\";done
done


echo '#3 copy ReleaseFile'
\cp -rf \$basePath/Publish/ReleaseFile/Station/. \"\$publishPath\"


echo '#4 copy ServiceCenter'
mkdir -p \"\$basePath/Publish/release/release/ServiceCenter(\$netVersion)\"
\cp -rf \$publishPath/ServiceCenter/. \"\$basePath/Publish/release/release/ServiceCenter(\$netVersion)/ServiceCenter\"
\cp -rf \"\$publishPath/01.ServiceCenter.bat\" \"\$basePath/Publish/release/release/ServiceCenter(\$netVersion)\"
\cp -rf \"\$publishPath/01.Start-4580.bat\" \"\$basePath/Publish/release/release/ServiceCenter(\$netVersion)\"


"



echo 'publish succeed£¡'





