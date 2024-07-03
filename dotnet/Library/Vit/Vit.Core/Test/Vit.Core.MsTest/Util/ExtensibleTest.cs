using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Util.Extensible;

namespace Vit.Core.MsTest.Util
{
    [TestClass]
    public class ExtensibleTest
    {

        [TestMethod]
        public void Test()
        {
            var ext = new Extensible();


            #region (x.1) SetData GetData
            {
                var obj = new object();

                ext.SetData("obj", obj);
                Assert.AreEqual(obj.GetHashCode(), ext.GetData<object>("obj").GetHashCode());
            }
            #endregion

            #region (x.2) GetDataByConvert
            {
                var obj = new object();

                ext.SetData("double", 12.123);
                Assert.AreEqual(12.123, ext.GetDataByConvert<double>("double"));
            }
            #endregion


            #region (x.3) GetDataBySerialize
            {
                var obj = new object();

                ext.SetData("ext", new { hello = "world" });
                Assert.AreEqual("world", ext.GetDataBySerialize<ModelA>("ext").hello);
            }
            #endregion
        }


        class ModelA
        {
            public string hello;
        }

    }
}
