using Sers.Core.Module.Rpc;
using Vit.Ioc;

namespace Vit.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static class RpcFactoryExtensions
    {
        static bool RpcContextEventAdded = false;
        public static void UseIoc(this RpcFactory data)
        {
            if (RpcContextEventAdded) return;

            RpcContextEventAdded = true;
            RpcFactory.AddRpcContextEvent(IocHelp.CreateScope);
        }

    }
}
