set -e

# bash 30.nuget-pack.sh


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "

if [ ! $NUGET_PATH ]; then NUGET_PATH=$basePath/Publish/release/.nuget; fi

mkdir -p $basePath/Publish/release/release

#----------------------------------------------
echo "30.nuget-pack.sh"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $NUGET_PATH:/root/.nuget \
-v $basePath:/root/code \
serset/dotnet:sdk-6.0 \
bash -c "

publishPath=/root/code/Publish/release/release/nuget

cd /root/code
for file in \$(grep -a '<pack>nuget</pack>' . -rl --include *.csproj)
do
	echo pack \$file
	mkdir -p \$publishPath
	cd /root/code
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet pack --configuration Release --output \"\$publishPath\"
done
"


 