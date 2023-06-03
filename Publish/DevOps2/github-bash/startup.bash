set -e

# cd /root/temp/svn/Publish/DevOps2/github-bash;bash startup.bash;

#---------------------------------------------------------------------
# args
args_="

export APPNAME=xxxxxx

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxxxxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxx

export GIT_SSH_SECRET=xxxxxx

# "

#----------------------------------------------
# cur path
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/temp/svn



#---------------------------------------------- 
echo '#1 build'
cd $basePath/Publish/DevOps2/build-bash; bash startup.bash;
cd $basePath/Publish/DevOps2/build-bash; bash 40.Station-publish-multiple.bash;


#---------------------------------------------- 
echo '#2 release-bash'
cd $basePath/Publish/DevOps2/release-bash; bash startup.bash;
 


#----------------------------------------------
echo "#3 get version" 
export version=`grep '<Version>' $(grep '<pack>\|<publish>' ${basePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`
echo $version



#----------------------------------------------
echo "#4 bash"
cd $curPath
for file in *.sh
do
    echo "-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")]" bash $file
    bash $file
done







