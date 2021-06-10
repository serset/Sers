set -e

# bash 30.nuget-build.sh


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/docker/jenkins/workspace/sqler/svn 



export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`

export name=Vit.Ioc
export projectPath=Vit.Ioc

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx

# "

 
 


 

#----------------------------------------------
echo "(x.2)nuget-构建包并推送到nuget server"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "cd /root/code/$projectPath
dotnet build --configuration Release
dotnet pack --configuration Release --output '/root/code/Publish/nuget'

# push to nuget server
for file in /root/code/Publish/nuget/*.nupkg ; 
do
    echo nuget push \$file
    dotnet nuget push \$file -k ${NUGET_KEY} -s ${NUGET_SERVER}
done
" 




