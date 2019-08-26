using Sers.Gover.Core.Model;
using Sers.ServiceCenter;
using System.Collections.Generic;
using System.Linq;

namespace Sers.Core.Extensions
{
    public static partial class ServiceStationExtensions
    {

       
        public static EServiceStationStatus? Status_Get(this Sers.ServiceCenter.ServiceStation data)
        {
            return data?.GetDataByConvert<EServiceStationStatus?>("Status");
        }
        public static void Status_Set(this Sers.ServiceCenter.ServiceStation data, EServiceStationStatus value)
        {
            data?.SetData("Status", value);
        }

        public static int ActiveApiNodeCount_Get(this Sers.ServiceCenter.ServiceStation data)
        {
            return data.apiNodes.Count((apiNode) => apiNode.Status_Get() == EServiceStationStatus.正常);
        }

        public static List<string> ApiStationNames_Get(this Sers.ServiceCenter.ServiceStation data)
        {
            return data.apiNodes.Select((apiNode) => apiNode.apiDesc.GetApiStationName()).Distinct().ToList();
        }


    }
}
