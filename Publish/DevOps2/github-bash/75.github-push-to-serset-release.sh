set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export appVersion=1.0-preview

export APPNAME=xxxxxx

export GIT_SSH_SECRET=xxxxxx

# "





#----------------------------------------------
echo "github-push release file to repo serset/release"
# releaseFile=$basePath/Publish/release/release-zip

# ssh key
echo "${GIT_SSH_SECRET}" > $basePath/Publish/release/serset
chmod 600 $basePath/Publish/release/serset

# push to github
docker run -i --rm \
-v $basePath/Publish/release:/root/release serset/git-client bash -c "
set -e
ssh-agent bash -c \"
ssh-add /root/release/serset
ssh -T git@github.com -o StrictHostKeyChecking=no
git config --global user.email 'serset@yeah.com'
git config --global user.name 'lith'
mkdir -p /root/code
cd /root/code
git clone git@github.com:serset/release.git /root/code
mkdir -p /root/code/file/${APPNAME}/${APPNAME}-${appVersion}
\\cp -rf  /root/release/release-zip/. /root/code/file/${APPNAME}/${APPNAME}-${appVersion}
git add /root/code/file/${APPNAME}/${APPNAME}-${appVersion}/.
git commit -m 'auto commit ${appVersion}'
git push -u origin master \" "


 
 
