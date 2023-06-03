set -e

# cd /root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code/Publish/DevOps/jenkins-bash; bash 01.svn-update.sh

#---------------------------------------------------------------------
# args
args_="
export SVN_USERNAME=jenkins
export SVN_PASSWORD=xxxxxx
# "


#---------------------------------------------------------------------
# cur path
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code



#---------------------------------------------------------------------
#3 cleanup
docker run -i --rm -v $basePath:/root/svn serset/svn-client svn cleanup /root/svn --remove-unversioned


#4 revert
docker run -i --rm -v $basePath:/root/svn serset/svn-client svn revert /root/svn -R


#5 push latest code
docker run -i --rm -v $basePath:/root/svn serset/svn-client svn update /root/svn --username "$SVN_USERNAME" --password "$SVN_PASSWORD" --no-auth-cache



cd $curPath
