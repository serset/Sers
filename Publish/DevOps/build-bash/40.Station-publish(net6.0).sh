set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

# "

curPath=$PWD


#----------------------------------------------
echo "(x.2)netVersion"
netVersion=net6.0



#---------------------------------------------- 
echo "(x.3)publish $netVersion"

#修改csproj文件中的版本号
cd $basePath
sed -i 's/net5.0/'"$netVersion"'/g'  `grep -a '<publish>' . -rl --include *.csproj`

cd $curPath
bash 40.Station-publish.sh;


#还原csproj文件中的版本号为net5.0
cd $basePath
sed -i 's/'"$netVersion"'/net5.0/g'  `grep -a '<publish>' . -rl --include *.csproj`


cd $curPath

