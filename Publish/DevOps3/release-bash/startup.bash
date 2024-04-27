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
curPath="$PWD"

cd "$curPath/../../.."
export basePath="$PWD"
cd "$curPath"

# export basePath=/root/temp/svn



#----------------------------------------------
echo "#1 get appVersion"
cd "$curPath/../build-bash"; source 19.get-app-version.bash;






#----------------------------------------------
echo "#2 bash"
cd $curPath
for file in *.sh
do
    echo "-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")] sh $file"
    sh "$file"
done


cd "$curPath"




