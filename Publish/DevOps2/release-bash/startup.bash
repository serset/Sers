set -e

# cd /root/temp/svn/Publish/DevOps/release-bash;bash startup.bash;

#---------------------------------------------------------------------
#(x.1)参数
args_="

export APPNAME=xxxxxx

export DOCKER_ImagePrefix=serset/
export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx
# "



#----------------------------------------------
#(x.2)当前路径
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/temp/svn



#----------------------------------------------
echo "(x.3)get appVersion"
export appVersion=`grep '<Version>' $(find ${basePath} -name *.csproj -exec grep '<pack>\|<publish>' -l {} \; | head -n 1) | grep -oE '\>(.*)\<' | tr -d '<>/'`
echo appVersion: $appVersion






#----------------------------------------------
#(x.4)bash

for file in *.sh
do
    echo "\n\n\n\n\n-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")] sh $file"
    sh $file
done







