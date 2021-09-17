using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Extensions;

namespace Vit.Core.MsTest.Extensions
{

    [TestClass]
    public class TimeStamp_Extensions_Test
    {
        [TestMethod]
        public void Test()
        {
            var time = "2021-01-01 12:00:00.123";     
            
            var timestamp = DateTime.Parse(time).ToTimeStamp();          

            Assert.AreEqual(1609502400123, timestamp);

            Assert.AreEqual(time, timestamp.TimeStampToDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));        

        }


         

    }
}
