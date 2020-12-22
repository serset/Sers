using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
namespace Vit.Core.MsTest.Util.ConfigurationManager
{
    [TestClass]
    public class ConfigurationManagerTest
    {

        public class AA
        {
            public string String;
        }

        [TestMethod]
        public void Test()
        {
            var Instance= Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance;

            #region (x.1)DateTime 
            var date = Instance.Get<DateTime>("Test", "DateTime");
            Assert.AreEqual(date, DateTime.Parse("2010-12-12 15:12"));

            var date2 = Instance.Get<DateTime>("Test", "DateTime1");
            Assert.AreEqual(date2, default(DateTime));

            var date3 = Instance.Get<DateTime?>("Test", "DateTime1");
            Assert.IsNull(date3);
            #endregion

            //(x.2)string
            var str = Instance.Get<string>("Test", "String");
            Assert.AreEqual(str, "String");


            //(x.3)long
            long l = Instance.Get<long>("Test","long");
            Assert.AreEqual(l, 1234567890123L);

            //(x.4)double
            double d = Instance.Get<double>("Test", "double");
            Assert.AreEqual(d, 1234567890123.45D);

            //(x.5)float
            float f = Instance.Get<float>("Test", "float");
            Assert.AreEqual(f, 123.456F);


            //(x.6)int
            int ti = Instance.Get<int>("Test", "int");
            Assert.AreEqual(ti, 123456);



            //(x.7)Model
            var m = Instance.Get<AA>("Test");
            Assert.IsNotNull(m);
            Assert.IsNotNull(m.String);

            //(x.8)object string
            var strTest = Instance.Get<string>("Test");
            Assert.IsNotNull(strTest);

        }
    }
}
