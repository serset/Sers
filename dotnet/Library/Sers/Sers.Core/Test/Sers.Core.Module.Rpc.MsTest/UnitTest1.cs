using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                using (var rpcContext = new RpcContext())
                {
                    var rpcData = new RpcContextData();
                    rpcContext.rpcData = rpcData;
                    rpcData.http.method = "POST";


                    var rpcData2 = RpcContextData.FromBytes(rpcData.ToBytes());


                    Assert.AreEqual(rpcData.http.method, rpcData2.http.method);
                }

            }
            catch
            {
                Assert.Fail();
            }



        }


    }
}
