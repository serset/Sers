@echo off

::(x.1)获取basePath
set curPath=%cd%
cd /D %~dp0
cd /d ../..
set basePath=%cd%
set nugetPath=%cd%\Doc\Publish\nuget

::(x.2)查找所有需要发布nuget的项目并发布
for /f "delims=" %%f in ('findstr /M /s /i "<pack/>" *.csproj') do (
	echo pack %basePath%\%%f\..
	cd /d %basePath%\%%f\.. 
	dotnet build --configuration Release
	dotnet pack --configuration Release --output %nugetPath%
	@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
)

cd /d %curPath%

echo 'pack nuget succeed！'
echo 'pack nuget succeed！'

 