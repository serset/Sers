using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Object_Serialize_Extensions;

namespace Vit.Core.MsTest.Module
{
    [TestClass]
    public class Serialization_Extensions_Test
    {
        class ModelA
        {
            public int? id;
            public string name;
            public DateTime time;
        }


        static readonly string testString = "testString中文12—=￥$《》<> \n\r\t😀" + DateTime.Now;

        [TestMethod]
        public void TestMethod()
        {

            var modelA = new ModelA { id = 1, name = testString, time = DateTime.Now };

            // #1 object <--> String 
            Assert.AreEqual(testString, modelA.Serialize().Deserialize<ModelA>()?.name);


            // #2 object <--> bytes
            Assert.AreEqual(modelA.SerializeToBytes().DeserializeFromBytes<ModelA>()?.name, testString);


            // #3 ConvertBySerialize
            var obj_ori = new { id = 1, name = testString, time = DateTime.Now };
            Assert.AreEqual(obj_ori.ConvertBySerialize<ModelA>()?.name, testString);


        }





    }
}
