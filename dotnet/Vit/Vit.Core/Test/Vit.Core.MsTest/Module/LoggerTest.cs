using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Core.MsTest.Module
{
    [TestClass]
    public class LoggerTest
    {
        [TestMethod]
        public void Logger_Test()
        {
            //会将日志写入 /Logs/{yyyy-MM}/{yyyy-MM-dd}Info.log
            Logger.Info("hello world!");
            Logger.Info(new { err = "err" });


            //会将日志写入 /Logs/{yyyy-MM}/{yyyy-MM-dd}Error.log
            Logger.Error("hello world!");
            Logger.Error(new Exception("hello world!"));
            Logger.Error("error",new Exception("hello world!"));
            Logger.Error(new SsError { errorCode = 404, errorMessage = "hello world!", errorTag = "150721_lith_1" });
            Logger.Error("error",new SsError { errorCode = 404, errorMessage = "hello world!", errorTag = "150721_lith_1" });

                                        

        }
    }
}
