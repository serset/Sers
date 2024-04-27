using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;

namespace Vit.Core.MsTest.Module
{
    [TestClass]
    public class Serialization_Newtonsoft_Test
    {
        class Person
        {
            public int? id;
            public string name;
            public DateTime birth;
            public ESex sex;
        }

        enum ESex
        {
            unknow,
            man,
            woman
        }

        static readonly string testString = "testString中文12—=￥$《》<> \n\r\t😀" + DateTime.Now;
        static Serialization_Newtonsoft Instance => Serialization_Newtonsoft.Instance;

        [TestMethod]
        public void TestMethod_String()
        {

            // #1 ValueType
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

            // #2 enum
            {
                TestBySerialize(ESex.man);
                TestBySerialize((ESex?)ESex.man);
                TestBySerialize((ESex?)null);

                var sex = Instance.Deserialize<ESex>("\"man\"");
                Assert.AreEqual(ESex.man, sex);
            }

            // #3 ModelA
            {
                var date = DateTime.Parse("2019-01-01 01:01:01");
                var expected = new Person { id = 1, name = testString, sex = ESex.man, birth = date };

                var str = Instance.Serialize(expected);
                var actual = Instance.Deserialize<Person>(str);


                Assert.AreEqual(expected.id, actual.id);
                Assert.AreEqual(expected.name, actual.name);
                Assert.AreEqual(expected.sex, actual.sex);
                Assert.AreEqual(expected.birth, actual.birth);
            }

            // #4 object
            {
                var date = DateTime.Parse("2019-01-01 01:01:01");
                var expected = new { id = 1, name = testString, sex = ESex.man, birth = date };

                var str = Instance.Serialize(expected);
                var actual = Instance.Deserialize<Person>(str);

                Assert.AreEqual(expected.id, actual.id);
                Assert.AreEqual(expected.name, actual.name);
                Assert.AreEqual(expected.sex, actual.sex);
                Assert.AreEqual(expected.birth, actual.birth);
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
            var objA = new { birth = date };


            var dateFormatString = Instance.serializeSetting.DateFormatString;
            try
            {
                var format = "yyyy-MM-dd";
                Instance.serializeSetting.DateFormatString = format;

                string str = Instance.Serialize(objA);
                var obj2 = Instance.Deserialize<Person>(str);

                Assert.AreEqual(DateTime.Parse("2019-01-01"), obj2.birth);
            }
            finally
            {
                Instance.serializeSetting.DateFormatString = dateFormatString;
            }
        }




        [TestMethod]
        public void TestMethod_Bytes()
        {
            // #1 Model
            {
                var date = DateTime.Parse("2019-01-01 01:01:01");
                var expected = new Person { id = 1, name = testString, sex = ESex.man, birth = date };

                var bytes = Instance.SerializeToBytes(expected);
                var actual = Instance.DeserializeFromBytes<Person>(bytes);


                Assert.AreEqual(expected.id, actual.id);
                Assert.AreEqual(expected.name, actual.name);
                Assert.AreEqual(expected.sex, actual.sex);
                Assert.AreEqual(expected.birth, actual.birth);
            }

            // #2 object
            {
                var date = DateTime.Parse("2019-01-01 01:01:01");
                var expected = new { id = 1, name = testString, sex = ESex.man, birth = date };

                var bytes = Instance.SerializeToBytes(expected);
                var actual = Instance.DeserializeFromBytes<Person>(bytes);


                Assert.AreEqual(expected.id, actual.id);
                Assert.AreEqual(expected.name, actual.name);
                Assert.AreEqual(expected.sex, actual.sex);
                Assert.AreEqual(expected.birth, actual.birth);
            }
        }


        [TestMethod]
        public void TestMethod_BytesToString()
        {
            //  bytes <--> String
            Assert.AreEqual(Instance.BytesToString(Instance.StringToBytes(testString)), testString);
        }




    }
}
