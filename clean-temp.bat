cd Publish\DevOps2\build-cmd
VsTool.exe delete --path "..\..\.." --file "*.suo|*.user" --directory "obj|bin|.vs|packages"


rd /s/q ..\..\release


echo %~n0.bat success£¡

pause