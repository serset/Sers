using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sers.Core.Module.Serialization.Text;

namespace Sers.Core.MsTest.Module
{
    [TestClass]
    public class Serialization_Text_Test
    {
        class ModelA
        {
            public int? id;
            public string name;
            public DateTime time;
        }
        static readonly string testString = "testString中文12—=￥$《》<> \n\r\t😀" + DateTime.Now;
        static Serialization_Text Instance => Serialization_Text.Instance;

        [TestMethod]
        public void TestMethod_String()
        {


            // #1 object
            {
                var modelA = new ModelA { id = 1, name = testString };
                var objA = new { id = 1, name = testString };


                Assert.AreEqual(testString, Instance.Deserialize<ModelA>(Instance.Serialize(modelA))?.name);
                Assert.AreEqual(testString, Instance.Deserialize<ModelA>(Instance.Serialize(objA))?.name);
            }

            // #2 ValueType
            {
                // string
                TestBySerialize(testString);
                TestBySerialize((string)null);

                // int
                TestBySerialize(10);
                TestBySerialize((int?)10);
                TestBySerialize((int?)null);

                // long
                TestBySerialize((long)123456789);
                TestBySerialize((long?)123456789);
                TestBySerialize((long?)null);

                // double
                TestBySerialize(10.4567);
                TestBySerialize((double?)10.4567);
                TestBySerialize((double?)null);

                // bool
                TestBySerialize(true, "true");
                TestBySerialize((bool?)false);
                TestBySerialize((bool?)null);

                // DateTime
                var date = DateTime.Parse("2019-01-01 01:01:01");
                TestBySerialize(date, "\"2019-01-01 01:01:01\"");
                TestBySerialize((DateTime?)date);
                TestBySerialize((DateTime?)null);
            }
        }

        public void TestBySerialize<T>(T value)
        {
            var str = Instance.Serialize(value);
            var actual = Instance.Deserialize<T>(str);
            Assert.AreEqual(value, actual);
        }
        public void TestBySerialize<T>(T value, string expected)
        {
            var str = Instance.Serialize(value);
            Assert.AreEqual(expected, str);
            var actual = Instance.Deserialize<T>(str);
            Assert.AreEqual(value, actual);
        }



        [TestMethod]
        public void TestMethod_DateTimeFormat()
        {
            var date = DateTime.Parse("2019-01-01 01:01:01");
            var objA = new { time = date };


            var dateFormatString = Instance.jsonConverter_DateTime.dateFormatString;
            try
            {
                var format = "yyyy-MM-dd";
                Instance.jsonConverter_DateTime.dateFormatString = format;

                string str = Instance.Serialize(objA);
                var obj2 = Instance.Deserialize<ModelA>(str);

                Assert.AreEqual(DateTime.Parse("2019-01-01"), obj2.time);
            }
            finally
            {
                Instance.jsonConverter_DateTime.dateFormatString = dateFormatString;
            }
        }




        [TestMethod]
        public void TestMethod_Bytes()
        {
            var modelA = new ModelA { id = 1, name = testString, time = DateTime.Now };
            var objA = new { id = 1, name = testString, time = DateTime.Now };

            Assert.AreEqual(Instance.DeserializeFromBytes<ModelA>(Instance.SerializeToBytes(modelA))?.name, testString);
            Assert.AreEqual(Instance.DeserializeFromBytes<ModelA>(Instance.SerializeToBytes(objA))?.name, testString);
        }



    }
}
