set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export APPNAME=Vit.Library
export appVersion=2.2.14

export WebDav_BaseUrl="https://nextcloud.xxx.com/remote.php/dav/files/release/releaseFiles/ki_jenkins"
export WebDav_User="username:pwd"

# "


# will not throw errors if WebDav service is not available


#---------------------------------------------------------------------
echo "78.push-releaseFiles-to-webdav.bash -> #1 create dir"

docker run -i --rm curlimages/curl sh -c "curl -X MKCOL -u \"$WebDav_User\" \"$WebDav_BaseUrl/$APPNAME\" " || true
docker run -i --rm curlimages/curl sh -c "curl -X MKCOL -u \"$WebDav_User\" \"$WebDav_BaseUrl/$APPNAME/$appVersion\" " || true

#---------------------------------------------------------------------
echo "78.push-releaseFiles-to-webdav.bash -> #2 push release files"

docker run -i --rm \
-v "$basePath/Publish/release/release-zip":/releaseFiles \
curlimages/curl \
sh -c "
cd /releaseFiles
for file in /releaseFiles/*
do
    echo ''
    echo '----------------------------'
    fileName=\"\${file##*/}\"
    echo push file: \$fileName
    curl -X PUT -u \"$WebDav_User\" -T "/releaseFiles/\$fileName" \"$WebDav_BaseUrl/$APPNAME/$appVersion/\$fileName\"
done
" || true

#---------------------------------------------------------------------
echo "78.push-releaseFiles-to-webdav.bash -> #3 success"
