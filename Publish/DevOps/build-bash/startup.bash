set -e

# cd /root/docker-data/dev/jenkins/jenkins_home/workspace/Repo/Sers/code/Publish/DevOps/build-bash;bash startup.bash;

#----------------------------------------------
# cur path 
curPath=$PWD

cd $curPath/../../..
export basePath=$PWD
cd $curPath





#----------------------------------------------
echo "build-bash/startup.bash"

for file in *.sh
do
    echo "\n\n\n\n\n-----------------------------------------------------------------"
    echo "[$(date "+%H:%M:%S")] sh $file"
    sh $file
done



 
#----------------------------------------------
#9
cd $curPath
