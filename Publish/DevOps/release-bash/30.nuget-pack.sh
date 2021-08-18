set -e

# bash 30.nuget-pack.sh


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

# "

mkdir -p $basePath/Publish/release/release/nuget
nugetPath=Publish/release/release/nuget



#----------------------------------------------
echo "(x.2)nuget-pack"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $basePath/Publish/release/.nuget:/root/.nuget \
-v $basePath:/root/code \
serset/dotnet:sdk-5.0 \
bash -c "
cd /root/code
for file in \$(grep -a '<pack>nuget</pack>' . -rl --include *.csproj)
do
	echo pack \$file
	cd /root/code
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet pack --configuration Release --output '/root/code/$nugetPath'
done
" 


 