using Sers.Core.Module.Rpc;
using Sers.Core.Util.Ioc;

namespace Sers.Core.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static class RpcFactoryExtensions
    {

        public static void UseIoc(this RpcFactory data)
        {
            RpcContext.AddEvent(IocHelp.CreateScope);
        }

    }
}
