set -e

# cd /root/temp/svn/Publish/DevOps/github-bash;bash startup.bash;



#----------------------------------------------
#(x.1)µ±Ç°Â·¾¶
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath


# export basePath=/root/temp/svn

export name=Sers

#export DOCKER_USERNAME=serset
#export DOCKER_PASSWORD=xxx

#export NUGET_SERVER=https://api.nuget.org/v3/index.json
#export NUGET_KEY=xxxxxxxxxx

#export export GIT_SSH_SECRET=xxxxxx


#----------------------------------------------
echo "(x.2)get version" 
export version=`grep '<Version>' $(grep '<pack/>\|<publish>' ${basePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`
echo $version



#----------------------------------------------
#(x.3)bash

echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
bash 00.release.sh

echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
bash 71.file-zip.sh

echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
bash 76.github-push-release.sh



