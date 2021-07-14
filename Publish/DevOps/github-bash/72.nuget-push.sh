set -e



#---------------------------------------------------------------------
#(x.1)参数
args_="

export basePath=/root/temp/svn

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx

# "


nugetPath=Publish/release/release/nuget
 

#----------------------------------------------
echo "(x.2)nuget-push"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $basePath:/root/code \
serset/dotnet:sdk-6.0 \
bash -c "
for file in /root/code/$nugetPath/*.nupkg
do
    echo nuget push \$file
    dotnet nuget push \$file -k ${NUGET_KEY} -s ${NUGET_SERVER}
done
" || true


 