set -e

# source 05.svn-merge.bash


#---------------------------------------------------------------------
# (x.1)参数
args_="
  export      codePath=/root/docker-cache/jenkins/jenkins_home/workspace/Sers/develop/20/code-with-prod-version
  export      SVN_PATH=svn://svn.ki.lith.cloud/Sers
  export      SVN_USERNAME=jenkins
  export      SVN_PASSWORD=**
  export      versionSuffix=.158
# "


docker run -i --rm -v $codePath:/root/svn \
-e codePath="$codePath" -e SVN_PATH="$SVN_PATH" -e SVN_USERNAME="$SVN_USERNAME" -e SVN_PASSWORD="$SVN_PASSWORD" -e versionSuffix="$versionSuffix" \
docker.lith.cloud:8/dockerhub/serset/svn-client bash -c '
set -e

# (x.2) change version in csproj and commit to develop branch
# (x.x.1) pull code from develop branch
codePath=/root/svn/branch-develop
mkdir -p $codePath;cd $codePath;
svn checkout $SVN_PATH/branches/develop $codePath --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache  > /dev/null;

# (x.x.2) change version in csproj
export versionSuffix=$versionSuffix
cd $codePath/Publish/DevOps/build-bash; source 20.change-app-version.bash; 
echo "appVersion: $appVersion"
cd $codePath;

# (x.x.3) commit to develop branch
svn commit $codePath -m "[tag] Sers $appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache




# (x.3) merge to trunk
# (x.x.1) pull code from trunk branch
codePath=/root/svn/branch-trunk
mkdir -p $codePath;cd $codePath;
svn checkout $SVN_PATH/trunk $codePath --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache > /dev/null;

# (x.x.2) merge to trunk
svn merge $SVN_PATH/branches/develop --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache

# (x.x.3) commit to trunk branch
svn commit $codePath -m "[trunk] merge from develop:$appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache



# (x.4) create tag branch from trunk
echo "appVersion: $appVersion"
svn copy $SVN_PATH/trunk $SVN_PATH/tags/2.1/$appVersion -m "[tag] Sers $appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache





# (x.5) change to next version in csproj and commit to develop branch
# (x.x.1) pull code from develop branch
codePath=/root/svn/branch-develop-next
mkdir -p $codePath;cd $codePath;
svn checkout $SVN_PATH/branches/develop $codePath --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache

# (x.x.2) change version in csproj
export versionSuffix="-temp"
cd $codePath/Publish/DevOps/build-bash; source 21.change-to-next-version.bash; 
echo "appVersion: $appVersion"
cd $codePath;

# (x.x.3) commit to develop branch
svn commit $codePath -m "[develop] $appVersion" --username $SVN_USERNAME --password $SVN_PASSWORD --no-auth-cache


'