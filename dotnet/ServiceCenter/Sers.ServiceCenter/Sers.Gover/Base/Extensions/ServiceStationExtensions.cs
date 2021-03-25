using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sers.Gover.Base.Model;
using Sers.ServiceCenter.Entity;

namespace Vit.Extensions
{
    public static partial class ServiceStationExtensions
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EServiceStationStatus? Status_Get(this ServiceStation data)
        {
            return data?.GetDataByConvert<EServiceStationStatus?>("Status");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Status_Set(this ServiceStation data, EServiceStationStatus value)
        {
            data?.SetData("Status", value);
        }

        public static int ActiveApiNodeCount_Get(this ServiceStation data)
        {
            return data.apiNodes.Count((apiNode) => apiNode.Status_Get() == EServiceStationStatus.正常);
        }

        public static List<string> ApiStationNames_Get(this ServiceStation data)
        {
            return data.apiNodes.Select((apiNode) => apiNode.apiDesc.ApiStationNameGet()).Distinct().OrderBy(m=>m).ToList();
        }


    }
}
