set -e

# cd /root/temp/svn/Publish/DevOps/release-bash;bash startup.bash;

#---------------------------------------------------------------------
# args
args_="

export APPNAME=xxxxxx

export DOCKER_ImagePrefix=serset/
export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx
# "



#----------------------------------------------
# cur path
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/temp/svn



#----------------------------------------------
echo "#1 get appVersion"
export appVersion=`grep '<Version>' $(find ${basePath} -name *.csproj -exec grep '<pack>\|<publish>' -l {} \; | head -n 1) | grep -oE '\>(.*)\<' | tr -d '<>/'`
echo appVersion: $appVersion






#----------------------------------------------
echo "#2 bash"

for file in *.sh
do
    echo "-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")] sh $file"
    sh $file
done







