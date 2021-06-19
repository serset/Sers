VsTool.exe delete --path "..\.." --file "*.suo|*.user" --directory "obj|bin|.vs|packages"


call "清理-发布文件.bat"

echo %~n0.bat 执行成功！

pause