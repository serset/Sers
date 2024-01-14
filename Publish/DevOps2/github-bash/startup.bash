set -e

# cd /root/temp/svn/Publish/DevOps2/github-bash;bash startup.bash;

#---------------------------------------------------------------------
# args
args_="

#export APPNAME=xxxxxx

export DOCKER_USERNAME=serset
export DOCKER_PASSWORD=xxxxxx

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxx

export WebDav_BaseUrl="https://nextcloud.xxx.com/remote.php/dav/files/release/releaseFiles/ki_jenkins"
export WebDav_User="username:pwd"

# "

#----------------------------------------------
# cur path
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

# export basePath=/root/temp/svn

if [ ! $APPNAME ]; then 
	export APPNAME=$(cat "$basePath/Publish/DevOps2/environment/env.APPNAME.txt" | tr -d '\n')
	echo "APPNAME: [${APPNAME}]" 
fi

#---------------------------------------------- 
echo '#1 build'
cd $basePath/Publish/DevOps2/build-bash; bash startup.bash;

if [ -f "$basePath/Publish/DevOps2/environment/github-bash__publish-multiple-netcore-version.txt" ]; then
	cd $basePath/Publish/DevOps2/build-bash; bash 40.Station-publish-multiple.bash;
fi

#---------------------------------------------- 
echo '#2 release-bash'
cd $basePath/Publish/DevOps2/release-bash; bash startup.bash;
 


#----------------------------------------------
echo "#3 get appVersion" 
cd $basePath/Publish/DevOps2/build-bash; source 19.get-app-version.bash;



#----------------------------------------------
echo "#4 bash"
cd $curPath
for file in *.sh
do
    echo "-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")]" sh $file
    sh $file
done







