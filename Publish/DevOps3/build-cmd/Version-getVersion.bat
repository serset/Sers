@echo off

:: get current version


:: #1 get csproj
for /f "delims=" %%a in ('findstr /M /s /i /r "<pack> <publish>" "..\..\..\*.csproj"') do set "csproj=%%~a"
echo %csproj%

:: #2 get version
for /f "tokens=3 delims=><" %%a in ('type %csproj%^|findstr "<Version>.*Version"') do set version=%%a

:: set version=2.1.3
:: echo  %version%


:: #3 get v1 v2 v3
for /f "tokens=1 delims=-" %%i in ("%version%") do set numVersion=%%i

:: v1 v2 v3
for /f "tokens=1 delims=." %%i in ("%numVersion%") do set v1=%%i
for /f "tokens=2 delims=." %%i in ("%numVersion%") do set v2=%%i
for /f "tokens=3 delims=." %%i in ("%numVersion%") do set v3=%%i


:: #4 newVersion
::set /a v3=1+%v3%
set  newVersion=%v1%.%v2%.%v3%-preview
echo [%version%]-^>[%newVersion%]


pause