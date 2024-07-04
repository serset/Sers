using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Serialize_Extensions;

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
            Assert.AreEqual(testString,modelA.SerializeToBytes().DeserializeFromBytes<ModelA>()?.name );


            // #3 ConvertBySerialize
            {
                var obj_ori = new { id = 1, name = testString, time = DateTime.Now };
                Assert.AreEqual(testString, obj_ori.ConvertBySerialize<ModelA>()?.name);
            }
            {
                var str = new { id = 1, name = testString, time = DateTime.Now }.Serialize();
                Assert.AreEqual(testString, str.ConvertBySerialize<ModelA>()?.name);
            }
            {
                Assert.AreEqual(testString, testString.ConvertBySerialize<string>());
            }

        }





    }
}
