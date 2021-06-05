set -e

# cd /home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code/Sers/dotnet/Doc/DevOps; sh 20.nuget-publish-main.sh


#(x.1)当前路径
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/../../../../..
basePath=$PWD
 
# basePath=/home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers


echo "(x.2)构建dotnet项目并发布"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $basePath/code:/root/code \
-v $basePath/nuget/packages:/root/.nuget/packages \
serset/dotnet:6.0-sdk \
bash /root/code/Sers/dotnet/Doc/DevOps/21.nuget-publish.sh 







cd $curWorkDir


