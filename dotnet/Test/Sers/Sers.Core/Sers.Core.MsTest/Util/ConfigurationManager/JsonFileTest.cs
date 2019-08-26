using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Util.ConfigurationManager;
using static Sers.Core.Util.ConfigurationManager.ConfigurationManager;

namespace Sers.Core.MsTest.Util.ConfigurationManager
{
    [TestClass]
    public class JsonFileTest
    { 
        [TestMethod]
        public void Test()
        { 
      
            var file = new JsonFile("Data","SersConfig.json");
            file.Root.ValueSetByPath("3", "a1", "a2", "a3");
            file.SaveToFile();


            new JsonFile("Data", "SersConfig.json").SetValueByPath("value","path1","path2");

        }
    }
}
