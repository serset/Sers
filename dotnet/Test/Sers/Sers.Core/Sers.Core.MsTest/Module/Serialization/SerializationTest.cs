using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sers.Core.Module.Api.RouteMap;
using Sers.Core.Extensions;

namespace Sers.Core.MsTest.Module.Serialization
{ 
    [TestClass]
    public class SerializationTest
    {

        

        [TestMethod]
        public void TestMethod1()
        {
            var obj = new { Date = DateTime.Parse("2019-01-01 01:01:01"),obj=new { Date2 = DateTime.Parse("2019-02-02 01:01:01") } };

            string str = obj.Serialize();

            Sers.Core.Module.Serialization.Serialization.Instance.SetDateTimeFormat("yyyy-MM-dd");

            string str2 = obj.Serialize();

            Assert.AreEqual("sss","sss");
 




        }


    }
}
