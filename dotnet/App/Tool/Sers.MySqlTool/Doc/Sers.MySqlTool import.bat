cd /d Sers.MySqlTool
 
dotnet Sers.MySqlTool.dll import "Data Source=lj.sersms.com;Port=7006;Database=db_test_import2;SslMode=none;User Id=root;Password=123456;CharSet=utf8;" "Data\MysqlBackup\db_test.db"  "-truncate"
pause

 
 