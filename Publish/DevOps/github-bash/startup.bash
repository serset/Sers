set -e

# cd /root/temp/svn/Publish/DevOps/github-bash;bash startup.bash;



#----------------------------------------------
#(x.1)µ±Ç°Â·¾¶
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath


# export basePath=/root/temp/svn

export name=ServiceAdaptor

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

for file in *.sh
do
    echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    echo bash $file
    bash $file
done







