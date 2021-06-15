set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet

export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`

export export GIT_SSH_SECRET=xxxxxx

export name=Sers


# "

 



#----------------------------------------------
echo "(x.2.1)发布文件-创建文件夹及内容"

mkdir -p $codePath/Doc/Publish/release

 

echo 1.创建 nuget-Sers
/cp -rf  $codePath/Doc/Publish/nuget $codePath/Doc/Publish/release/nuget

echo 2.创建 SersPublish
/cp -rf  $codePath/Doc/Publish/SersPublish $codePath/Doc/Publish/release/SersPublish

echo 3.创建 CL压测
/cp -rf  $codePath/Doc/Publish/CL压测 $codePath/Doc/Publish/release/CL压测

echo 4.创建 Sers压测
/cp -rf  $codePath/Doc/Publish/Sers压测 $codePath/Doc/Publish/release/Sers压测

echo 5.创建 docker制作镜像Sers
/cp -rf  $codePath/Doc/Publish/SersDocker/docker制作镜像Sers $codePath/Doc/Publish/release/docker制作镜像Sers

echo 6.创建 docker部署Sers
/cp -rf  $codePath/Doc/Publish/SersDocker/docker部署Sers $codePath/Doc/Publish/release/docker部署Sers




echo "(x.2.3)发布文件-压缩" 
docker run --rm -i \
-v $codePath/Publish:/root/file \
serset/filezip dotnet FileZip.dll zip -i /root/file/Doc/Publish/release -o /root/file/Doc/Publish/${name}-${version}.zip





#----------------------------------------------
echo "(x.3)github-提交release文件到release仓库"
# releaseFile=$codePath/Doc/Publish/Sers-$version.zip

#复制ssh key
echo "${GIT_SSH_SECRET}" > $codePath/Publish/git/serset
chmod 600 $codePath/Publish/git/serset

#推送到github
docker run -i --rm -v $codePath/Doc/Publish/${name}-${version}.zip:/root/${name}-${version}.zip serset/git-client bash -c "
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
cp /root/${name}-${version}.zip /root/code/file/${name}
git add file/${name}/${name}-${version}.zip
git commit -m 'auto commit ${version}'
git push -u origin master \" "





 
 
