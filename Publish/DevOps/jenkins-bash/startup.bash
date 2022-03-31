set -e


# cd /root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code/Publish/DevOps/jenkins-bash; bash startup.bash;


#---------------------------------------------------------------------
#(x.1)参数
args_="

export SVN_USERNAME=jenkins
export SVN_PASSWORD=xxxxxx

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxxxxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxx

# "

export name=Sers


#----------------------------------------------
#(x.2)当前路径 
curWorkDir=$PWD

cd $curWorkDir/../../..
export codePath=$PWD
cd $curWorkDir



#----------------------------------------------
#(x.3)svn-update
bash 01.svn-update.sh;


 

#---------------------------------------------- 
echo '(x.4)release-bash'
cd $codePath/Publish/DevOps/release-bash;bash startup.bash;

 
 
#----------------------------------------------
#(x.9)
#cd $curWorkDir
