set -e


#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export NUGET_PATH=$basePath/Publish/release/.nuget

# "


curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath

#----------------------------------------------
echo "40.Station-publish-multiple.bash"

for netVersion in net5.0 netcoreapp3.1 netcoreapp3.0 netcoreapp2.2 netcoreapp2.1
do
	echo "#2.1 publish netVersion: $netVersion"

	echo "#2.2 change netVersion to $netVersion in csproj"
	sed -i 's/net6.0/'"$netVersion"'/g' `find ${basePath} -name *.csproj -exec grep '<publish>' -l {} \;`


	echo "#2.3 sh 40.Station-publish.sh"
	cd $curPath
	sh 40.Station-publish.sh


	echo "#2.4 change back netVersion to net6.0 in csproj"
	sed -i 's/'"$netVersion"'/net6.0/g' `find ${basePath} -name *.csproj -exec grep '<publish>' -l {} \;`

done



cd $curPath



