set -e

#  cd $codePath/Doc/DevOps/github; bash 40.dotnet-publish.sh;

#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet

# "

 

#---------------------------------------------- 
echo "(x.2)publish netcoreapp2.1"

export netVersion=netcoreapp2.1 
bash $codePath/Doc/DevOps/github/41.dotnet-publish-by-netVersion.sh;




#---------------------------------------------- 
echo "(x.3)publish net6.0"

#修改csproj文件中的版本号为6.0
cd $codePath
sed -i 's/netcoreapp2.1/net6.0/g'  `grep -a 'netcoreapp2.1' . -rl --include *.csproj`

export netVersion=net6.0
bash $codePath/Doc/DevOps/github/41.dotnet-publish-by-netVersion.sh;


#修改csproj文件中的版本号为2.1
cd $codePath
sed -i 's/net6.0/netcoreapp2.1/g'  `grep -a 'net6.0' . -rl --include *.csproj`