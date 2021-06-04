set -e

# cd /home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code/Sers/dotnet/Doc/DevOps; sh 01.svn-update.sh


#(x.1)当前路径
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/../../../..
codePath=$PWD
# codePath=/home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code

#(x.2)cleanup
docker run -i --rm -v $codePath:/root/svn serset/svn-client svn cleanup /root/svn --remove-unversioned


#(x.3)revert
docker run -i --rm -v $codePath:/root/svn serset/svn-client svn revert /root/svn -R




#(x.4)拉取最新代码
docker run -i --rm -v $codePath:/root/svn serset/svn-client svn update /root/svn --username lith --password beyourself --no-auth-cache



cd $curWorkDir
