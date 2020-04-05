using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vit.Extensions;

namespace Sers.Core.Module.Rpc.MsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                using (var rpcContext = RpcFactory.CreateRpcContext())
                {
                    var rpcData = RpcFactory.CreateRpcContextData();
                    rpcContext.rpcData = rpcData;
                    rpcData.http_method_Set("POST");


                    var rpcData2 = RpcFactory.CreateRpcContextData();
                    rpcData2.UnpackOriData(rpcData.PackageOriData());
                }
                 
            }
            catch (Exception ex)
            {              
                Assert.Fail();
            }



        }

      
    }
}
