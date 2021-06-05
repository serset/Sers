@echo off

::获取当前版本号
:: set version=2.1.3.356 
for /f "tokens=3 delims=><" %%a in ('type ..\..\Library\Vit\Vit.Core\Vit.Core\Vit.Core.csproj^|findstr "<Version>.*Version"') do set version=%%a

:: v1 v2 v3
for /f "tokens=1 delims=." %%i in ("%version%") do set v1=%%i
for /f "tokens=2 delims=." %%i in ("%version%") do set v2=%%i
for /f "tokens=3 delims=." %%i in ("%version%") do set v3=%%i


:: 获取最新版本号
:: set v4=356 
for /f "tokens=4 delims= " %%i in ('svn info "svn://svn.sers.cloud/Sers2.1"^|findstr "Rev:"') do set v4=%%i

:: set /a v3=1+%v3%
set /a v4=1+%v4%
set  newVersion=%v1%.%v2%.%v3%.%v4%

 
echo 自动修改版本号 [%version%]-^>[%newVersion%]
echo.

:: 调用工具 替换csproj文件中的版本号
VsTool.exe replace -r --path "..\.." --file "*.csproj" --old "%version%" --new "%newVersion%"
VsTool.exe replace -r --path "..\.." --file "packages.config" --old "%version%" --new "%newVersion%"

:: 调用工具 替换docker镜像命令中的版本号
VsTool.exe replace -r --path "..\..\Doc\PublishFile\SersDocker" --file "*.txt" --old "%version%" --new "%newVersion%"


echo.
echo.
echo.
echo 已经成功修改版本号 [%version%]-^>[%newVersion%]
pause