using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sers.Core.Module.Log.MsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LoggerTest()
        {
            for (int t = 0; t < 10; t++)
            {
                Logger.Info("safsfsaf");


                Logger.Debug("Debug");
            }

        }
    }
}
