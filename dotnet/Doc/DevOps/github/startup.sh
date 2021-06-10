set -e

#----------------------------------------------
#(x.1)当前路径 
curWorkDir=$PWD

cd $curWorkDir/../../..
export codePath=$PWD
cd $curWorkDir



export name=sqler
export projectPath=Sqler

#export DOCKER_USERNAME=serset
#export DOCKER_PASSWORD=xxx

#export NUGET_SERVER=https://api.nuget.org/v3/index.json
#export NUGET_KEY=xxxxxxxxxx

#export export GIT_SSH_SECRET=xxxxxx






#----------------------------------------------
echo "(x.2)get version" 
export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`
# echo $version







#----------------------------------------------
echo "(x.3)自动发布 $name-$version"

for file in *.sh
do
    if [[ $file != "startup.sh" ]]
    then
        sh $file
    fi
done






 
#----------------------------------------------
#(x.9)
#cd $curWorkDir
