set -e

#---------------------------------------------------------------------
# args
args_="

export basePath=/root/temp/svn
export netVersion=net6.0
export publishPath="$basePath/Publish/release/release/Station($netVersion\)"

# "



#----------------------------------------------
echo '#4 copy extra release files'

mkdir -p "$basePath/Publish/release/release/ServiceCenter($netVersion)"
cp -rf $publishPath/ServiceCenter/. "$basePath/Publish/release/release/ServiceCenter($netVersion)/ServiceCenter"
cp -rf "$publishPath/01.ServiceCenter.bat" "$basePath/Publish/release/release/ServiceCenter($netVersion)"
cp -rf "$publishPath/01.Start-4580.bat" "$basePath/Publish/release/release/ServiceCenter($netVersion)"





