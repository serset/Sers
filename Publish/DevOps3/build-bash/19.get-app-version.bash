set -e

# export versionSuffix='.1234.preview'
# bash 19.get-app-version.bash


#---------------------------------------------------------------------
# args
args_="

export versionSuffix='  '

# "

# remove spaces
versionSuffix=${versionSuffix// /}

#----------------------------------------------
# basePath
if [ -z "$basePath" ]; then basePath=$PWD/../../..; fi



#----------------------------------------------
echo "#1 get appVersion"
# get csproj file with appVersion tag, if not exist get file with pack or publish tag
csprojPath=$(find ${basePath} -name *.csproj -exec grep '<appVersion>' -l {} \; | head -n 1);
if [ ! -f "$csprojPath" ]; then csprojPath=$(find ${basePath} -name *.csproj -exec grep '<pack>\|<publish>' -l {} \; | head -n 1);  fi
if [ -f "$csprojPath" ]; then export appVersion=`grep '<Version>' "$csprojPath" | grep -oE '\>(.*)\<' | tr -d '<>/'`;  fi
echo "appVersion from csproj: $appVersion"

# get v1 v2 v3
v1=$(echo $appVersion | tr '.' '\n' | sed -n 1p)
v2=$(echo $appVersion | tr '.' '\n' | sed -n 2p)
v3=$(echo $appVersion | tr '.-' '\n' | sed -n 3p)


#export nextAppVersion="${appVersion%%-*}$versionSuffix"
export nextAppVersion="$v1.$v2.$v3$versionSuffix"
echo "nextAppVersion: $nextAppVersion"

