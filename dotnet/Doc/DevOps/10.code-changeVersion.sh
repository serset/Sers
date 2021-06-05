set -e

# cd /home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code/Sers/dotnet/Doc/DevOps; bash 10.changeVersion.sh


#(x.1)当前路径
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/../../../..
codePath=$PWD 
# codePath=/home/DataStore/HDD/Data/008.jenkins/data/PersistentVolume/workspace/Sers/code


echo "(x.2)get oldVersion"
#oldVersion=1.1.0.53
oldVersion=`grep '<Version>' Sers/dotnet/Library/Vit/Vit.Core/Vit.Core/Vit.Core.csproj | grep -o '[0-9\.]\+'`

vs=(${oldVersion//./ }) 



echo "(x.3)get newVersion"
v4=`docker run -i --rm -v $codePath:/root/svn serset/svn-client svn info \
| grep 'Rev:' | awk -v RS='\r\n' '{print $4}'` 
 
newVersion=${vs[0]}.${vs[1]}.${vs[2]}.$v4 

 


echo "(x.4)modify csproj"
echo "$oldVersion -> $newVersion"
cd $codePath/Sers/dotnet
sed -i "s/$oldVersion/$newVersion/g" `grep -a $oldVersion . -rl --include *.csproj`




cd $curWorkDir


