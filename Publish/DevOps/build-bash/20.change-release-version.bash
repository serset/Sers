set -e

# export versionSuffix='.1234.preview'
# bash 20.change-release-version.bash


#---------------------------------------------------------------------
#(x.1)参数
args_="

export versionSuffix=''

# "


#----------------------------------------------
#(x.2)当前路径 
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath



#----------------------------------------------
echo "(x.3)get version" 
export version=`grep '<Version>' $(grep '<pack>\|<publish>' ${basePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`
echo "version: $version"


export releaseVersion="${version%%-*}$versionSuffix"
echo "releaseVersion: $releaseVersion"


#----------------------------------------------
if [ -n "$versionSuffix" ]; then
	echo "(x.4) change release version from [$version] to [$releaseVersion]" 
	 
	cd $basePath
	sed -i 's/'"$version"'/'"$releaseVersion"'/g'  `grep -a '<pack>\|<publish>' . -rl --include *.csproj`
fi




#----------------------------------------------
#(x.9)
cd $curPath