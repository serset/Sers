set -e

#  cd $codePath/Publish/DevOps/github; bash 40.dotnet-publish.sh;

#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn

# "

 

#---------------------------------------------- 
echo "(x.2)publish netcoreapp2.1"

export netVersion=netcoreapp2.1 
bash $codePath/Publish/DevOps/github/41.dotnet-publish-by-netVersion.bash;




#---------------------------------------------- 
echo "(x.3)publish net6.0"

#修改csproj文件中的版本号为6.0
cd $codePath
sed -i 's/netcoreapp2.1/net6.0/g'  `grep -a '<publish>' . -rl --include *.csproj`

export netVersion=net6.0
bash $codePath/Publish/DevOps/github/41.dotnet-publish-by-netVersion.bash;


#修改csproj文件中的版本号为2.1
cd $codePath
sed -i 's/net6.0/netcoreapp2.1/g'  `grep -a '<publish>' . -rl --include *.csproj`