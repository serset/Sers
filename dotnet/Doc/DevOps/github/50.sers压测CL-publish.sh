set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet

# "

 


#----------------------------------------------
echo "(x.2)dotnet-publish"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
set -e

echo 'publish Client'
cd /root/code/Library/Sers/Sers.CL/Test/CommunicationManage/CmClient
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/CL压测/CmClient

echo 'publish Server'
cd /root/code/Library/Sers/Sers.CL/Test/CommunicationManage/CmServer
dotnet build --configuration Release
dotnet publish --configuration Release --output /root/code/Doc/Publish/CL压测/CmServer     
" 








