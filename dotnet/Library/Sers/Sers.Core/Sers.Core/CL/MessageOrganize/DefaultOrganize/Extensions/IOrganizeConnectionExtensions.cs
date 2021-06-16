using Sers.Core.CL.MessageDelivery;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;

namespace Vit.Extensions
{
    public static partial class IOrganizeConnectionExtensions
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static IDeliveryConnection GetDeliveryConn(this IOrganizeConnection data)
        {
            return (data as OrganizeConnection)?.deliveryConn;
        }
    }
}
