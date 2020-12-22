using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions;

namespace Sers.Core.Module.LocalApi.MsTest.LocalApi
{
    [TestClass]
    public class LocalApiTest
    {
        [TestMethod]
        public void TestCall()
        {
            try
            {          
 
                //(x.1)构建
                LocalApiService localApiService = new LocalApiService() { workThreadCount =1};
                localApiService.LoadSersApi(this.GetType().Assembly);

                try
                {
                    localApiService.Start();


                    string route;
                    object argValue;
                    object returnValue;

                    //(x.2)调用
                    route = "/Test/api/GetDeviceGuidList";
                    string arg = "asfsdf";
                    argValue = new { arg };

                    returnValue = localApiService.CallLocalApi<string>(route, argValue);
                    Assert.AreEqual(arg + "Test", returnValue);

                    //(x.3)调用
                    route = "/Test/api/getList";
                    argValue = new
                    {
                        arr = new string[] { "a", "b" }
                    };
                    returnValue = localApiService.CallLocalApi<string>(route, argValue);
                    var list = localApiService.CallLocalApi<List<string>>(route, argValue);
                    Assert.AreEqual(list.Count, 2);
                    string[] arr = localApiService.CallLocalApi<string[]>(route, argValue);
                    Assert.AreEqual(arr.Length, 2);

                }
                finally
                {
                    localApiService.Stop();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }

        }

    }
}
