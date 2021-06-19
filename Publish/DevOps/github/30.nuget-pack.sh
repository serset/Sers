set -e

# bash 30.nuget-pack.sh


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn

# "

mkdir -p $codePath/Publish/release/release/nuget
nugetPath=Publish/release/release/nuget



#----------------------------------------------
echo "(x.2)nuget-pack"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
cd /root/code
for file in \$(grep -a '<pack/>' . -rl --include *.csproj)
do
	echo pack \$file
	cd /root/code
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet pack --configuration Release --output '/root/code/$nugetPath'
done
" 


 