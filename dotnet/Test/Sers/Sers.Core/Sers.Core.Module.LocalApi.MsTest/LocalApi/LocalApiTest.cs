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

          
 
                LocalApiService localApiService = new LocalApiService();
                localApiService.UseSsApiDiscovery();


                localApiService.Discovery(this.GetType().Assembly);

                
                string route;
                object argValue;
                object returnValue;

                route = "/Test/api/GetDeviceGuidList";
                string arg = "asfsdf";
                argValue = new { arg};

                returnValue = localApiService.CallLocalApi<string>(route, argValue);
                Assert.AreEqual(arg + "Test", returnValue);


                route = "/Test/api/getList";
                argValue = new {
                    arr=new string[] {"a","b" }
                };
                returnValue = localApiService.CallLocalApi<string>(route, argValue);
                var list = localApiService.CallLocalApi<List<string>>(route, argValue);
                Assert.AreEqual(list.Count, 2);
                string[] arr = localApiService.CallLocalApi<string[]>(route, argValue);
                Assert.AreEqual(arr.Length,2);


            }
            catch (Exception ex)
            {
                Assert.Fail();
            }

        }

    }
}
