using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Vit.Core.Module.Serialization;
using Vit.Extensions;

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
            string testString = "testString中文12—=￥$《》<> \n\r\t😀"+ DateTime.Now;

            var modelA = new ModelA { id=1,name = testString, time=DateTime.Now };

            #region (x.1)bytes <--> String
            Assert.AreEqual(Serialization.Instance.BytesToString(Serialization.Instance.StringToBytes(testString)), testString);
            Assert.AreEqual(testString.StringToBytes().BytesToString(), testString);
            #endregion


            #region (x.2)object <--> String
            Assert.AreEqual(Serialization.Instance.DeserializeFromString<ModelA>(Serialization.Instance.SerializeToString(modelA))?.name, testString);
            Assert.AreEqual(modelA.Serialize().Deserialize<ModelA>()?.name, testString);
            #endregion


            #region (x.3)object <--> bytes
            Assert.AreEqual(Serialization.Instance.DeserializeFromBytes<ModelA>(Serialization.Instance.SerializeToBytes(modelA))?.name, testString);
            Assert.AreEqual(modelA.SerializeToBytes().DeserializeFromBytes<ModelA>()?.name, testString);
            #endregion


            #region (x.4)object <--> ArraySegmentByte
            Assert.AreEqual(Serialization.Instance.DeserializeFromArraySegmentByte<ModelA>(Serialization.Instance.SerializeToArraySegmentByte(modelA))?.name, testString);
            Assert.AreEqual(modelA.SerializeToArraySegmentByte().DeserializeFromArraySegmentByte<ModelA>()?.name, testString);
            #endregion

            #region (x.5)ConvertBySerialize
            var obj_ori = new { id = 1, name = testString, time = DateTime.Now };

            Assert.AreEqual(Serialization.Instance.ConvertBySerialize<ModelA>(obj_ori)?.name, testString);
            Assert.AreEqual(obj_ori.ConvertBySerialize<ModelA>()?.name, testString);
            #endregion


            #region (x.6)DateTimeFormat           

            var obj = new {
                Date = DateTime.Parse("2019-01-01 01:01:01"),
                obj =new { Date2 = DateTime.Parse("2019-02-02 01:01:01") }
            };

            string str = obj.Serialize();

            Serialization_Newtonsoft.Instance.SetDateTimeFormat("yyyy-MM-dd");

            string str2 = obj.Serialize();
            var jtObj = str2.Deserialize<JObject>();

            Assert.AreEqual(jtObj.StringGetByPath("Date"), "2019-01-01");
            Assert.AreEqual(jtObj.StringGetByPath("obj", "Date2"), "2019-02-02");

            #endregion



        }


    }
}
