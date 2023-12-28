set -e

# bash 30.nuget-pack.sh


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "

if [ ! $NUGET_PATH ]; then NUGET_PATH=$basePath/Publish/release/.nuget; fi


nugetPath=Publish/release/release/nuget
mkdir -p $basePath/Publish/release/release



#----------------------------------------------
echo "30.nuget-pack.sh"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $NUGET_PATH:/root/.nuget \
-v $basePath:/root/code \
serset/dotnet:sdk-6.0 \
bash -c "
cd /root/code
for file in \$(grep -a '<pack>nuget</pack>' . -rl --include *.csproj)
do
	echo pack \$file
	mkdir -p /root/code/$nugetPath
	cd /root/code
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet pack --configuration Release --output '/root/code/$nugetPath'
done
"


 