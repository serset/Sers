::获取当前版本号
:: set version=2.1.1.356 
for /f "tokens=3 delims=><" %%a in ('type ..\..\dotnet\Library\Sers\Sers.Core\Sers.Core\Sers.Core.csproj^|findstr "<Version>.*Version"') do set version=%%a



echo ["%version%"]


echo 创建 Sers-%version%.zip
dotnet FileZip/FileZip.dll zip -p -i "../release/release" -o "../release/Sers-%version%.zip" 


echo %~n0.bat 执行成功！

:: pause

