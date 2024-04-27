set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export APPNAME=xxxx
export appVersion=xxxx

export WebDav_BaseUrl="https://nextcloud.xxx.com/remote.php/dav/files/release/releaseFiles/ki_jenkins"
export WebDav_User="username:pwd"

# "





#----------------------------------------------
if [ -z "$WebDav_BaseUrl" ]; then
	echo "github skip pushing release file to WebDav because invalid WebDav endpoint"
else
	echo "github push release file to WebDav"
	bash "$PWD/../release-bash/78.push-releaseFiles-to-webdav.bash";
fi


