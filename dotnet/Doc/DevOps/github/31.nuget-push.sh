set -e



#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn/dotnet

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx

# "


nugetPath=Doc/Publish/nuget
 

#----------------------------------------------
echo "(x.2)nuget-push"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
for file in /root/code/$nugetPath/*.nupkg
do
    echo nuget push \$file
    #dotnet nuget push \$file -k ${NUGET_KEY} -s ${NUGET_SERVER}
done
" || true


 