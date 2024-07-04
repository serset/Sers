cd Publish\DevOps3\build-cmd
VsTool.exe delete --path "..\..\.." --file "*.suo|*.user" --directory "obj|bin|.vs|packages|TestResults"


rd /s/q ..\..\release


echo %~n0.bat success£¡

:: pause