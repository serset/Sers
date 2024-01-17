set -e



#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn

export NUGET_SERVER=https://api.nuget.org/v3/index.json
export NUGET_KEY=xxxxxxxxxx

# "

#----------------------------------------------
echo "72.nuget-push.sh"

if [ ! -d "$basePath/Publish/release/release/nuget" ]; then
    echo '71.file-zip.sh -> skip for no nuget files exist'
    exit 0
fi


docker run -i --rm \
--env LANG=C.UTF-8 \
-v $basePath:/root/code \
serset/dotnet:sdk-6.0 \
bash -c "
for file in /root/code/Publish/release/release/nuget/*.nupkg
do
    echo nuget push \$file
    dotnet nuget push \$file -k ${NUGET_KEY} -s ${NUGET_SERVER} --skip-duplicate
done
" || true


 