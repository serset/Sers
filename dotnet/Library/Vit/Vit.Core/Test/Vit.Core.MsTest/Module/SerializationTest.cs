using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json.Linq;

using Vit.Core.Module.Serialization;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Vit.Core.MsTest.Module
{
    [TestClass]
    public class SerializationTest
    {
        class ModelA
        {
            public int? id;
            public string name;
            public DateTime time;
        }


        [TestMethod]
        public void TestMethod()
        {
            string testString = "testString中文12—=￥$《》<> \n\r\t😀" + DateTime.Now;

            var modelA = new ModelA { id = 1, name = testString, time = DateTime.Now };

            #region (x.1)bytes <--> String      
            Assert.AreEqual(testString.StringToBytes().BytesToString(), testString);
            #endregion


            #region (x.2)object <--> String
            Assert.AreEqual(Json.DeserializeFromString<ModelA>(Json.SerializeToString(modelA))?.name, testString);
            Assert.AreEqual(modelA.Serialize().Deserialize<ModelA>()?.name, testString);
            #endregion


            #region (x.3)object <--> bytes
            Assert.AreEqual(Json.DeserializeFromBytes<ModelA>(Json.SerializeToBytes(modelA))?.name, testString);
            Assert.AreEqual(modelA.SerializeToBytes().DeserializeFromBytes<ModelA>()?.name, testString);
            #endregion


            #region (x.5)ConvertBySerialize
            var obj_ori = new { id = 1, name = testString, time = DateTime.Now };

            Assert.AreEqual(obj_ori.ConvertBySerialize<ModelA>()?.name, testString);
            #endregion


            #region (x.6)DateTimeFormat           

            var obj = new
            {
                Date = DateTime.Parse("2019-01-01 01:01:01"),
                obj = new { Date2 = DateTime.Parse("2019-02-02 01:01:01") }
            };

            string str = obj.Serialize();

            Serialization_Newtonsoft.Instance.serializeSetting.DateFormatString = "yyyy-MM-dd";

            string str2 = obj.Serialize();
            var jtObj = str2.Deserialize<JObject>();

            Assert.AreEqual(jtObj.StringGetByPath("Date"), "2019-01-01");
            Assert.AreEqual(jtObj.StringGetByPath("obj", "Date2"), "2019-02-02");

            #endregion



        }



        [TestMethod]
        public void TestMethod_DateTime()
        {
            var time = DateTime.Now;
            var str = Json.SerializeToString(time);
            Assert.AreEqual(time.ToString("yyyy-MM-dd HH:mm:ss"), str);
        }


    }
}
