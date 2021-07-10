@echo off

::(x.1)获取codePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../../..
set codePath=%cd%
set nugetPath=%codePath%/Publish/release/release/nuget

::(x.2)查找所有需要发布nuget的项目并发布
for /f "delims=" %%f in ('findstr /M /s /i "<pack/>" *.csproj') do (
	echo pack %codePath%\%%f\..
	cd /d "%codePath%\%%f\.."
	dotnet build --configuration Release
	dotnet pack --configuration Release --output "%nugetPath%"
	@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
)


echo %~n0.bat 执行成功！


cd /d "%curPath%"
