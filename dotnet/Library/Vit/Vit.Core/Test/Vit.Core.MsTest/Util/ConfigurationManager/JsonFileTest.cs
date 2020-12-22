using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Extensions;
using Vit.Core.Util.ConfigurationManager;

namespace Vit.Core.MsTest.Util.ConfigurationManager
{
    [TestClass]
    public class JsonFileTest
    { 
        [TestMethod]
        public void Test()
        {
            //set
            {
                var file = new JsonFile("Data", "SersConfig.json");

                file.root.ValueSetByPath(1, "p1", "p2", "p3");
                file.SaveToFile();

                file.Set("v4", "p1", "p2", "p4");

                file.SetByPath("v5", "p1.p2.p5");
            }

            //get
            {
                var file = new JsonFile("Data", "SersConfig.json");
                Assert.AreEqual(1, file.GetByPath<int>("p1.p2.p3"));
                Assert.AreEqual(1, file.Get<int>("p1", "p2", "p3"));

                Assert.AreEqual("v4", file.GetByPath<string>("p1.p2.p4"));
                Assert.AreEqual("v5", file.GetByPath<string>("p1.p2.p5"));
            }
        }
    }
}
