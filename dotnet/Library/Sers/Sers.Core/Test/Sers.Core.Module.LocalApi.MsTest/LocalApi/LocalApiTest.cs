using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.LocalApi.MsTest.LocalApi.Extensions;
using Vit.Extensions;

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
                LocalApiService localApiService = LocalApiServiceFactory.CreateLocalApiService() as LocalApiService;
                localApiService.threadCount = 4;
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
                    Assert.AreEqual(2,list.Count);
                    string[] arr = localApiService.CallLocalApi<string[]>(route, argValue);
                    Assert.AreEqual(2,arr.Length);

                }
                finally
                {
                    localApiService.Stop();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

    }
}
