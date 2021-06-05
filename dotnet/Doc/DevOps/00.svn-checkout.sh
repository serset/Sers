set -e

# cd /home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code/Sers/dotnet/Doc/DevOps; bash 00.svn-checkout.sh


#(x.1)当前路径
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/..
basePath=$PWD
# basePath=/home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers


#(x.2)创建文件夹
mkdir -p $basePath/code
chmod 777 $basePath/code

mkdir -p $basePath/nuget
chmod 777 $basePath/nuget



#(x.3)从svn拉取code
# svnServer=svn://sers.cloud
# svnUser=lith
# svnPwd=pwd
docker run -it --rm -v $basePath/code:/root/svn serset/svn-client svn checkout $svnServer /root/svn --username $svnUser --password $svnPwd --no-auth-cache

 
cd $curWorkDir