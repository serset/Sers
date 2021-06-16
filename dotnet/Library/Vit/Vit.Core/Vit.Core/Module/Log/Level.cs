using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Core.Module.Log
{
    /*
OFF > FATAL > ERROR > WARN > INFO > DEBUG  > ALL 

高于等级设定值方法（如何设置参见“配置文件详解”）都能写入日志， Off所有的写入方法都不写到日志里，ALL则相反。例如当我们设成Info时，logger.Debug就会被忽略而不写入文件，但是FATAL,ERROR,WARN,INFO会被写入，因为他们等级高于INFO。

在具体写日志时，一般可以这样理解日志等级：

FATAL（致命错误）：记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。

ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。

WARN（警告）：记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。

INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。

DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。
         
         
         */

    /// <summary>
    /// FATAL > ERROR > WARN > INFO > DEBUG 
    /// </summary>
    public enum Level
    {
        OFF = 0, FATAL = 1, ERROR = 2, WARN = 3, INFO = 4, DEBUG = 5, ALL = 6,ApiTrace=7
    }
}
