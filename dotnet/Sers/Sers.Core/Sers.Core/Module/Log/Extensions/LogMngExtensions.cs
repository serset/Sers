using Sers.Core.Module.Log;
using Sers.Core.Util.ConfigurationManager;
using System;

namespace Sers.Core.Extensions
{
    public static partial class LogMngExtensions
    { 

        public static void InitByConfigurationManager(this LogMng data)
        {
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.Logger.PrintToConsole"))
            {
                Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };
            }
        }



    }
}
