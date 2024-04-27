set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "

if [ ! $NUGET_PATH ]; then NUGET_PATH=$basePath/Publish/release/.nuget; fi


#----------------------------------------------
echo "#40.Station-publish.sh -> find projects and build"

export devOpsPath="$PWD/.."

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $NUGET_PATH:/root/.nuget \
-v "$basePath":/root/code \
-v "$basePath":"$basePath" \
serset/dotnet:sdk-6.0 \
bash -c "
set -e

if grep '<publish>' -r --include *.csproj /root/code; then
	echo '#40.Station-publish.sh -> got projects need to be built'
else
	echo '#40.Station-publish.sh -> skip for no project needs to be built'
	exit 0
fi

echo '#1 get netVersion'
export netVersion=\$(grep '<TargetFramework>' \$(grep '<publish>' -rl --include *.csproj /root/code | head -n 1) | grep -oP '>(.*)<' | tr -d '<>')
echo netVersion: \$netVersion


export basePath=/root/code
export publishPath=\$basePath/Publish/release/release/Station\(\$netVersion\)
mkdir -p \$publishPath

echo '#2 publish station'
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


#3 copy station release files
if [ -d \"\$basePath/Publish/ReleaseFile/Station\" ]; then
	echo '#3 copy station release files'
	\cp -rf \$basePath/Publish/ReleaseFile/Station/. \"\$publishPath\"
fi


#4 copy extra release files
bashFile=\"$devOpsPath/environment/build-bash__40.Station-publish__#4_copyExtraReleaseFiles.sh\"
if [ -f \"\$bashFile\" ]; then
	echo '#4 copy extra release files'
	sh \"\$bashFile\"
fi




"



echo '#40.Station-publish.sh -> success!'





