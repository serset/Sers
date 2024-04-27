set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export appVersion=1.0-preview

export APPNAME=xxxxxx

# "


 


#---------------------------------------------------------------------
#2 init environment for github release



echo "appName=${APPNAME}" >> $GITHUB_ENV

echo "release_name=${APPNAME}-${appVersion}" >> $GITHUB_ENV
echo "release_tag=${appVersion}" >> $GITHUB_ENV

echo "release_draft=false" >> $GITHUB_ENV
echo "release_prerelease=false" >> $GITHUB_ENV

echo "release_body=${APPNAME} ${appVersion}" >> $GITHUB_ENV


echo "release_dirPath=${basePath}/Publish/release/release-zip" >> $GITHUB_ENV
echo "release_version=${appVersion}" >> $GITHUB_ENV

#filePath=$basePath/Publish/release/release-zip/Sers-ServiceCenter(net6.0)-${appVersion}.zip
#fileType="${filePath##*.}"
#echo "release_assetPath=${filePath}" >> $GITHUB_ENV
#echo "release_assetName=${APPNAME}-${appVersion}.${fileType}" >> $GITHUB_ENV
#echo "release_contentType=application/zip" >> $GITHUB_ENV


# draft or preivew
if [ "preview" = "$(echo $appVersion | tr -d \"0-9\-\\.\")" ]
then
  echo preivew
  echo "release_prerelease=true" >> $GITHUB_ENV
else
  if  [ "" = "$(echo $appVersion | tr -d \"0-9\-\\.\")" ]
  then
    echo release
  else
    echo draft
    echo "release_draft=true" >> $GITHUB_ENV
  fi
fi

