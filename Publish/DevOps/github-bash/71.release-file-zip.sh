set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

export version=`grep '<Version>' $(grep '<pack/>\|<publish>' ${basePath} -r --include *.csproj -l | head -n 1) | grep -oP '>(.*)<' | tr -d '<>'`

export name=ServiceAdaptor

# "

 



#----------------------------------------------
echo "压缩发布文件"

docker run --rm -i \
-v $basePath:/root/code \
serset/filezip filezip zip -p -i /root/code/Publish/release/release -o /root/code/Publish/release/${name}-${version}.zip 



