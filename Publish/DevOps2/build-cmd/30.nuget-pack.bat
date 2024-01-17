@echo off

::1 get basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../../..
set basePath=%cd%
set nugetPath=%basePath%/Publish/release/release/nuget

::2 find nuget projects and pack
for /f "delims=" %%f in ('findstr /M /s /i "<pack>nuget</pack>" *.csproj') do (
	echo pack %basePath%\%%f\..
	cd /d "%basePath%\%%f\.."
	dotnet build --configuration Release
	dotnet pack --configuration Release --output "%nugetPath%"
	@if errorlevel 1 (echo . & echo .  & echo error & pause) 
)


echo %~n0.bat success


cd /d "%curPath%"
