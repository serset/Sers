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
echo "#1 get version" 
export version=`grep '<Version>' $(find ${basePath} -name *.csproj -exec grep '<pack>\|<publish>' -l {} \; | head -n 1) | grep -oE '\>(.*)\<' | tr -d '<>/'`
echo "version from csproj: $version"

# get v1 v2 v3
v1=$(echo $version | tr '.' '\n' | sed -n 1p)
v2=$(echo $version | tr '.' '\n' | sed -n 2p)
v3=$(echo $version | tr '.-' '\n' | sed -n 3p)


#export appVersion="${version%%-*}$versionSuffix"
export appVersion="$v1.$v2.$v3$versionSuffix"
echo "appVersion: $appVersion"



#----------------------------------------------
#9
cd $curPath