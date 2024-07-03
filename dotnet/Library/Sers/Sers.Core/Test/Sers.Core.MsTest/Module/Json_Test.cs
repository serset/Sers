using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sers.Core.Module.Serialization.Text;

using Vit.Core.Module.Serialization;

namespace Sers.Core.MsTest.Module
{
    [TestClass]
    public class Json_Test
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
            Json.Instance = Serialization_Text.Instance;

            var modelA = new ModelA { id = 1, name = testString, time = DateTime.Now };

            // #0 null string
            {
                ModelA model = null;
                Assert.AreEqual("null", Json.Serialize(model));
                Assert.AreEqual(null, Json.Deserialize<ModelA>((string)null));
                Assert.AreEqual(null, Json.Deserialize<ModelA>("  "));
                Assert.AreEqual(null, Json.Deserialize<ModelA>("null"));
            }

            // #1 object <--> String
            {
                var str = Json.Serialize(modelA);
                var model = Json.Deserialize<ModelA>(str);
                Assert.AreEqual(testString, model?.name);
            }

            // #2 object <--> bytes
            {
                var bytes = Json.SerializeToBytes(modelA);
                var model = Json.DeserializeFromBytes<ModelA>(bytes);
                Assert.AreEqual(testString, model?.name);
            }


            // #3 ConvertBySerialize
            {
                var obj_ori = new { id = 1, name = testString, time = DateTime.Now };
                var model = Json.ConvertBySerialize<ModelA>(obj_ori);
                Assert.AreEqual(testString, model?.name);
            }

        }





    }
}
