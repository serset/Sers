set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn
export netVersion=netcoreapp2.1 
# "

 

#----------------------------------------------
echo "(x.2)dotnet-publish"
echo "dotnet version: ${netVersion}"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
set -e

publishPath=/root/code/Publish/Publish/SersPublish/$netVersion

#(x.3)查找所有需要发布的项目并发布
cd /root/code/dotnet
for file in \$(grep -a '<publish>' . -rl --include *.csproj)
do
	cd /root/code/dotnet
	
	#get name
	name=\`grep '<publish>' \$file -r | grep -oP '>(.*)<' | tr -d '<>'\`
	echo publish \$name

	#publish
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet publish --configuration Release --output \$publishPath/\$name

	#copy xml
	for filePath in bin/Release/$netVersion/*.xml ; do \\cp -rf \$filePath \$publishPath/\$name;done
done


#(x.4)copy bat
\\cp -rf /root/code/Publish/PublishFile/SersPublish/. \$publishPath


"




echo 'publish succeed！'






