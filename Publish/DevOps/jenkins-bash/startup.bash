set -e


# cd /root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code/Publish/DevOps/jenkins-bash; bash startup.bash;

#---------------------------------------------------------------------
#(x.1)参数
args_="
export APPNAME=xxxxxx

export SVN_USERNAME=jenkins
export SVN_PASSWORD=xxxxxx

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxxxxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxx

export NUGET_PATH=/root/docker-data/dev/jenkins/jenkins_home/workspace/.nuget

# "

#----------------------------------------------
#(x.2)当前路径 
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/temp/svn



#----------------------------------------------
echo '(x.3)svn-update'
bash 01.svn-update.sh;


 

#---------------------------------------------- 
echo '(x.4)build'
cd $basePath/Publish/DevOps/build-bash; bash startup.bash;


#---------------------------------------------- 
echo '(x.5)release-bash'
cd $basePath/Publish/DevOps/release-bash; bash startup.bash;
 



#----------------------------------------------
#(x.9)
#cd $curPath
