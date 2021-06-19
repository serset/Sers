set -e

# cd /root/temp/svn/Publish/DevOps/github;bash startup.bash;

#----------------------------------------------
#(x.1)当前路径 
curWorkDir=$PWD

cd $curWorkDir/../../..
export codePath=$PWD
cd $curWorkDir


# export codePath=/root/temp/svn
export name=Sers

#export DOCKER_USERNAME=serset
#export DOCKER_PASSWORD=xxx

#export NUGET_SERVER=https://api.nuget.org/v3/index.json
#export NUGET_KEY=xxxxxxxxxx

#export export GIT_SSH_SECRET=xxxxxx






#----------------------------------------------
echo "(x.2)get version" 
export version=`grep '<Version>' $(grep '<pack/>\|<publish>' ${codePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`
# echo $version







#----------------------------------------------
echo "(x.3)自动发布 $name-$version"

for file in *.sh
do
    echo %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    echo bash $file
    bash $file
done






 
#----------------------------------------------
#(x.9)
#cd $curWorkDir
