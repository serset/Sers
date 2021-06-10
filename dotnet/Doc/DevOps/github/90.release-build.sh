set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/docker/jenkins/workspace/sqler/svn 



export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`

export name=sqler

export export GIT_SSH_SECRET=xxxxxx

# "

 



#----------------------------------------------
echo "(x.2.1)发布文件-创建文件夹及内容"
mkdir -p $codePath/Publish/git
mkdir -p $codePath/Publish/release

cp -rf  $codePath/Publish/04.服务站点 $codePath/Publish/release/04.服务站点
cp -rf  $codePath/Publish/06.Docker $codePath/Publish/release/06.Docker
cp -rf  $codePath/Publish/06.Docker/制作镜像/${name}/app $codePath/Publish/release/04.服务站点/${name}


echo "(x.2.3)发布文件-压缩" 
docker run --rm -i \
-v $codePath/Publish:/root/file \
serset/filezip dotnet FileZip.dll zip -i /root/file/release -o /root/file/git/${name}-${version}.zip

 






#----------------------------------------------
echo "(x.3)github-提交release文件到release仓库"
# releaseFile=$codePath/Publish/git/${name}-${version}.zip

#复制ssh key
echo "${GIT_SSH_SECRET}" > $codePath/Publish/git/serset
chmod 600 $codePath/Publish/git/serset

#推送到github
docker run -i --rm -v $codePath/Publish/git:/root/git serset/git-client bash -c " \
set -e
ssh-agent bash -c \"
ssh-add /root/git/serset
ssh -T git@github.com -o StrictHostKeyChecking=no
git config --global user.email 'serset@yeah.com'
git config --global user.name 'lith'
mkdir -p /root/code
cd /root/code
git clone git@github.com:serset/release.git /root/code
mkdir -p /root/code/file/${name}
cp /root/git/${name}-${version}.zip /root/code/file/${name}
git add file/${name}/${name}-${version}.zip
git commit -m 'auto commit ${version}'
git push -u origin master \" "





 
 
