set -e

# bash 30.nuget-build.sh


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet

# "

 
 
nugetPath=Doc/Publish/nuget

 

#----------------------------------------------
echo "(x.2)nuget-pack"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
cd /root/code
for file in \$(grep -a '<TargetFramework>netstandard2.0</TargetFramework>' . -rl --include *.csproj)
do
    if ! [[ \$file =~ (Apm|Empty|Temp|Zmq|SharedMemory|\\-) ]]; then
        echo pack \$file
        cd /root/code
	cd \$(dirname \"\$file\")
        dotnet build --configuration Release
	dotnet pack --configuration Release --output '/root/code/$nugetPath'
    fi
done
" 


 