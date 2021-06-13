set -e


# cd /home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code/Sers/dotnet/Doc/DevOps/k8s; bash 99.jenkins.sh




#路径
codePath=/home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code




#(x.1)svn-update

echo '(x.1.1)svn-cleanup'
chroot /host bash -c "docker run -i --rm -v $codePath:/root/svn serset/svn-client svn cleanup /root/svn --remove-unversioned"

echo '(x.1.2)svn-revert'
chroot /host bash -c "docker run -i --rm -v $codePath:/root/svn serset/svn-client svn revert /root/svn -R"

echo '(x.1.3)svn-update'
# svnUser=lith
# svnPwd=pwd
chroot /host bash -c "docker run -i --rm -v $codePath:/root/svn serset/svn-client svn update /root/svn --username $svnUser --password $svnPwd --no-auth-cache"


 
 

 


 
echo '(x.2)code-changeVersion'
chroot /host bash -c "cd $codePath/Sers/dotnet/Doc/DevOps;bash 10.code-changeVersion.sh"


 
 
echo '(x.3)nuget-publish'
chroot /host bash -c "cd $codePath/Sers/dotnet/Doc/DevOps;bash 20.nuget-publish-main.sh"


 