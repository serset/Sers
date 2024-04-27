set -e

# cd build-bash; bash startup.bash;

#----------------------------------------------
# cur path
curPath=$PWD

cd $curPath/../../..
export basePath="$PWD"
cd $curPath





#----------------------------------------------
echo "build-bash/startup.bash"

for file in *.sh
do
    echo "-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")] sh $file"
    sh $file
done



 
#----------------------------------------------
#
cd $curPath
