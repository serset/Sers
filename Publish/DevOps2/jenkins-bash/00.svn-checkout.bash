
# args
args_="
basePath=/root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers
export SVN_PATH=svn://svn.ki.lith.cloud/Sers2.1/Sers
export SVN_USERNAME=jenkins
export SVN_PASSWORD=xxxxxx
# "

#2 create dir
mkdir -p $basePath/code
chmod 777 $basePath/code



#3 pull code from svn
docker run -it --rm -v $basePath/code:/root/svn serset/svn-client svn checkout "$SVN_PATH" /root/svn --username "$SVN_USERNAME" --password "$SVN_PASSWORD" --no-auth-cache

