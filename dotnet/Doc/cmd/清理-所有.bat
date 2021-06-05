VsTool.exe delete --path "..\.." --file "*.suo|*.user" --directory "obj|bin|.vs|packages"


call "清理-发布文件.bat"
 
::rd /s/q 生成压缩包\Publish

echo succeed
echo succeed
echo succeed

pause