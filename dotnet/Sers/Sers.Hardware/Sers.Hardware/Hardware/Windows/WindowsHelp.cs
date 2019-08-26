using Sers.Hardware.Hardware.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Sers.Core.Util.Hardware.DeviceManage;

namespace Sers.Hardware.Hardware.Ms
{
    public class WindowsHelp
    {


        public static IUsageReader GetUsageReader()
        {
            return new UsageReader();
        }


        //public readonly static UsageReader reader = new UsageReader();

        ///// <summary>
        ///// 读取CPU使用率信息
        ///// </summary>
        ///// <returns></returns>
        //public static float ReadCpuUsage()
        //{

        //    return (float)reader.ReadUsageInfo().cpuUsage;
        //    //return ServerConfig.ReadCpuUsage();
        //}

    }
}
