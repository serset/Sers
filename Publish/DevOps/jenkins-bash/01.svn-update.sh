set -e

# cd /root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code/Publish/DevOps/jenkins-bash; bash 01.svn-update.sh

#---------------------------------------------------------------------
#(x.1)参数
args_="
export SVN_USERNAME=jenkins
export SVN_PASSWORD=xxxxxx
# "


#---------------------------------------------------------------------
#(x.2)当前路径
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code



#---------------------------------------------------------------------
#(x.3)cleanup
docker run -i --rm -v $basePath:/root/svn serset/svn-client svn cleanup /root/svn --remove-unversioned


#(x.4)revert
docker run -i --rm -v $basePath:/root/svn serset/svn-client svn revert /root/svn -R


#(x.5)拉取最新代码
docker run -i --rm -v $basePath:/root/svn serset/svn-client svn update /root/svn --username "$SVN_USERNAME" --password "$SVN_PASSWORD" --no-auth-cache



cd $curPath
