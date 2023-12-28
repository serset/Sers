set -e

# source 07.svn-changeToNextTempVersion.bash


#---------------------------------------------------------------------
# args
args_="
  export      codePath=/root/docker-cache/jenkins/jenkins_home/workspace/Sers/develop/20/code-with-prod-version
  export      SVN_PATH=svn://svn.ki.lith.cloud/Sers
  export      SVN_USERNAME=jenkins
  export      SVN_PASSWORD=**
# "


docker run -i --rm -v $codePath:/root/svn \
-e codePath="$codePath" -e SVN_PATH="$SVN_PATH" -e SVN_USERNAME="$SVN_USERNAME" -e SVN_PASSWORD="$SVN_PASSWORD" \
docker.lith.cloud:8/dockerhub/serset/svn-client bash -c '
set -e


echo "07.svn-changeToNextTempVersion.bash"
echo "07.svn-changeToNextTempVersion.bash  -->  #1 pull code from develop branch"
codePath=/root/svn/branch-develop-next
mkdir -p $codePath;cd $codePath;
svn checkout $SVN_PATH/branches/develop $codePath --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache

echo "07.svn-changeToNextTempVersion.bash  -->  #2 change version in csproj"
export versionSuffix="-temp"
cd $codePath/Publish/DevOps2/build-bash; source 21.change-to-next-version.bash; 
echo "appVersion: [$appVersion] -> [$nextAppVersion]"
export appVersion=$nextAppVersion
cd $codePath;

echo "07.svn-changeToNextTempVersion.bash  -->  #3 commit to develop branch"
svn commit $codePath -m "[develop] $appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache


'