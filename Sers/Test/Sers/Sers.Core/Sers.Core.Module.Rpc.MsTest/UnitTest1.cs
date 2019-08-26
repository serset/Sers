using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.Rpc;
using Sers.Core.Util.Ioc;
using System;

namespace Sers.Rpc.MsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            App_Init();



        }

        public void App_Init()
        {
            IocHelp.AddScoped<IRpcContextData, RpcContextData>();
            IocHelp.Update();
        }

        public void Rpc_Init(ArraySegment<byte> oriRpcData,Action Invoke)
        {
            using (var rpcContext = RpcFactory.Instance.CreateRpcContext())
            using (var iocScope = IocHelp.CreateScope())
            {             
                var rpcData = RpcFactory.Instance.CreateRpcContextData();
                rpcData.UnpackOriData(oriRpcData);
                rpcContext.SetRpcContextData(rpcData);

                //do something here         
                Invoke();
            }


        }
    }
}
