using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

 using static  Sers.Core.Util.ConfigurationManager.ConfigurationManager;

namespace Sers.Core.MsTest.Util.ConfigurationManager
{
    [TestClass]
    public class ConfigurationManagerTest
    {

        public class AA
        {
            public string Redis;
        }

        [TestMethod]
        public void Test()
        {
            //DateTime
            var date = Instance.Get<DateTime>("Test", "DateTime");
            Assert.AreEqual(date, DateTime.Parse("2010-12-12 15:12"));

            var date2 = Instance.Get<DateTime>("Test", "DateTime1");
            Assert.AreEqual(date2, default(DateTime));

            var date3 = Instance.Get<DateTime?>("Test", "DateTime1");
            Assert.IsNull(date3);

            //string
            var redisStr = Instance.Get<string>("ConnectionStrings", "Redis");
            Assert.AreEqual(redisStr, "127.0.0.1,syncTimeout=3000,abortConnect=false");

            //Model
            var m = Instance.Get<AA>("ConnectionStrings");
            Assert.IsNotNull(m);

            //object string
            var ConnectionStrings = Instance.Get<string>("ConnectionStrings");
            Assert.IsNotNull(ConnectionStrings);

            //long
            long l= Instance.Get<long>("Test","long");
            Assert.AreEqual(l, 1234567890123L);

            //double
            double d = Instance.Get<double>("Test", "double");
            Assert.AreEqual(d, 1234567890123.45D);

            //float
            float f = Instance.Get<float>("Test", "float");
            Assert.AreEqual(f, 123.456F);


            //int
            int ti = Instance.Get<int>("Test", "int");
            Assert.AreEqual(ti, 123456);

        }
    }
}
