set -e

# export versionSuffix='.1234.preview'
# bash 21.change-to-next-version.bash


#---------------------------------------------------------------------
#(x.1)参数
args_="

export versionSuffix='  '

# "

# remove spaces
versionSuffix=${versionSuffix// /}

#----------------------------------------------
#(x.2)当前路径 
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
((v3++));


export appVersion="$v1.$v2.$v3$versionSuffix"
echo "appVersion: $appVersion"


#----------------------------------------------
echo "#2 change app version from [$version] to [$appVersion]" 
sed -i 's/'"$version"'/'"$appVersion"'/g'  `find ${basePath} -name *.csproj -exec grep '<pack>\|<publish>' -l {} \;`




#----------------------------------------------
#9
cd $curPath