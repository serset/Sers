using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sers.Core.Extensions;
using Sers.Core.Module.SersDiscovery;
using System;
using System.Collections.Generic;
using Sers.Core.Module.Api.LocalApi;

namespace Sers.Core.Module.Api.MsTest.LocalApi
{
    [TestClass]
    public class LocalApiTest
    {
        [TestMethod]
        public void TestCall()
        {
            try
            {

          
 
                LocalApiMng localApiMng = new LocalApiMng();
                localApiMng.UseSsApiDiscovery();


                localApiMng.Discovery(this.GetType().Assembly);

                
                string route;
                object argValue;
                object returnValue;

                route = "/Test/api/GetDeviceGuidList";
                string arg = "asfsdf";
                argValue = new { arg};

                returnValue = localApiMng.CallLocalApi<string>(route, argValue);
                Assert.AreEqual(arg + "Test", returnValue);


                route = "/Test/api/getList";
                argValue = new {
                    arr=new string[] {"a","b" }
                };
                returnValue = localApiMng.CallLocalApi<string>(route, argValue);
                var list = localApiMng.CallLocalApi<List<string>>(route, argValue);
                Assert.AreEqual(list.Count, 2);
                string[] arr = localApiMng.CallLocalApi<string[]>(route, argValue);
                Assert.AreEqual(arr.Length,2);


            }
            catch (Exception ex)
            {
                Assert.Fail();
            }

        }

    }
}
