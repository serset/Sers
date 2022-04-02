set -e

# cd /root/temp/svn/Publish/DevOps/github-bash;bash startup.bash;

#---------------------------------------------------------------------
#(x.1)参数
args_="

export APPNAME=xxxxxx

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxxxxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxx

export GIT_SSH_SECRET=xxxxxx

# "

#----------------------------------------------
#(x.2)当前路径
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/temp/svn



#---------------------------------------------- 
echo '(x.4)build'
cd $basePath/Publish/DevOps/build-bash; bash startup.bash;



#---------------------------------------------- 
echo '(x.5)release-bash'
cd $basePath/Publish/DevOps/release-bash; bash startup.bash;
 


#----------------------------------------------
echo "(x.3)get version" 
export version=`grep '<Version>' $(grep '<pack>\|<publish>' ${basePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`
echo $version



#----------------------------------------------
#(x.4)bash

for file in *.sh
do
    echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    echo "[$(date "+%H:%M:%S")]" bash $file
    bash $file
done







