set -e

# export versionSuffix='.1234.preview'
# bash 20.change-release-version.bash


#---------------------------------------------------------------------
# args
args_="

export versionSuffix='  '

# "

# remove spaces
versionSuffix=${versionSuffix// /}

#----------------------------------------------
# curPath
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath



#----------------------------------------------
echo "#1 get appVersion" 
export appVersion=`grep '<Version>' $(find ${basePath} -name *.csproj -exec grep '<appVersion>' -l {} \; | head -n 1) | grep -oE '\>(.*)\<' | tr -d '<>/'`
echo "appVersion from csproj: $appVersion"

# get v1 v2 v3
v1=$(echo $appVersion | tr '.' '\n' | sed -n 1p)
v2=$(echo $appVersion | tr '.' '\n' | sed -n 2p)
v3=$(echo $appVersion | tr '.-' '\n' | sed -n 3p)


#export nextAppVersion="${appVersion%%-*}$versionSuffix"
export nextAppVersion="$v1.$v2.$v3$versionSuffix"
echo "nextAppVersion: $nextAppVersion"



#----------------------------------------------
#9
cd $curPath