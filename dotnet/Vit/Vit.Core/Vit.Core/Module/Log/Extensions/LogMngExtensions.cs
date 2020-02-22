using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using System;

namespace Vit.Extensions
{
    public static partial class LogMngExtensions
    { 

        public static void InitByConfigurationManager(this LogMng data)
        {
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Vit.Logger.PrintToConsole"))
            {
                Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };
            }
        }



    }
}
