set -e

# source 06.svn-createReleaseBranch.bash


#---------------------------------------------------------------------
# args
args_="
  export      codePath=/root/docker-cache/jenkins/jenkins_home/workspace/Sers/develop/20/code-with-prod-version
  export      SVN_PATH=svn://svn.ki.lith.cloud/Sers
  export      SVN_USERNAME=jenkins
  export      SVN_PASSWORD=**
  export      versionSuffix=.158
# "

# remove spaces
versionSuffix=${versionSuffix// /}

docker run -i --rm -v $codePath:/root/svn \
-e codePath="$codePath" -e SVN_PATH="$SVN_PATH" -e SVN_USERNAME="$SVN_USERNAME" -e SVN_PASSWORD="$SVN_PASSWORD" -e versionSuffix="$versionSuffix" \
docker.lith.cloud:8/dockerhub/serset/svn-client bash -c '
set -e

echo "06.svn-createReleaseBranch.bash"
echo "06.svn-createReleaseBranch.bash  -->  #1 change version in csproj and commit to develop branch"
echo "06.svn-createReleaseBranch.bash  -->  #1.1 pull code from develop branch"
codePath=/root/svn/branch-develop
mkdir -p $codePath;cd $codePath;
svn checkout $SVN_PATH/branches/develop $codePath --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache  > /dev/null;

echo "06.svn-createReleaseBranch.bash  -->  #1.2 change version in csproj"
export versionSuffix=$versionSuffix
cd $codePath/Publish/DevOps2/build-bash; source 20.change-app-version.bash; 
echo "appVersion: [$appVersion] -> [$nextAppVersion]"
export appVersion=$nextAppVersion
cd $codePath;

echo "06.svn-createReleaseBranch.bash  -->  #1.3 commit to develop branch"
svn commit $codePath -m "[tag] Sers $appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache




echo "06.svn-createReleaseBranch.bash  -->  #2 merge to trunk"
echo "06.svn-createReleaseBranch.bash  -->  #2.1 pull code from trunk branch"
codePath=/root/svn/branch-trunk
mkdir -p $codePath;cd $codePath;
svn checkout $SVN_PATH/trunk $codePath --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache > /dev/null;

echo "06.svn-createReleaseBranch.bash  -->  #2.2 merge to trunk"
svn merge $SVN_PATH/branches/develop --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache

echo "06.svn-createReleaseBranch.bash  -->  #2.3 commit to trunk branch"
svn commit $codePath -m "[trunk] merge from develop:$appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache



echo "06.svn-createReleaseBranch.bash  -->  #3 create tag branch from trunk"
echo "appVersion: $appVersion"
svn copy $SVN_PATH/trunk $SVN_PATH/tags/$appVersion -m "[tag] Sers $appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache



'