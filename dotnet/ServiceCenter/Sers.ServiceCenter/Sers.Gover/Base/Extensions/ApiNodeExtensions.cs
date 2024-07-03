using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Sers.Gover.Base;
using Sers.Gover.Base.Model;
using Sers.ServiceCenter.Entity;

namespace Vit.Extensions
{
    public static partial class ApiNodeExtensions
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EServiceStationStatus Status_Get(this ApiNode data)
        {
            return data?.GetDataByConvert<EServiceStationStatus?>("Status") ?? EServiceStationStatus.暂停;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Status_Set(this ApiNode data, EServiceStationStatus value)
        {
            data?.SetData("Status", value);
        }



        public static void Stop(this ApiNode apiNode, ApiLoadBalancingMng apiLoadBalancingMng)
        {
            if (apiNode.Status_Get() == EServiceStationStatus.暂停)
            {
                return;
            }
            apiNode.Status_Set(EServiceStationStatus.暂停);
            apiLoadBalancingMng.ApiNode_Remove(apiNode);
        }

        public static void Start(this ApiNode apiNode, ApiLoadBalancingMng apiLoadBalancingMng)
        {
            if (apiNode.Status_Get() == EServiceStationStatus.正常)
            {
                return;
            }
            apiNode.Status_Set(EServiceStationStatus.正常);
            apiLoadBalancingMng.ApiNode_Add(apiNode);
        }

        #region reason
        static void StopReason_Add(this ApiNode data, string reason)
        {
            var reasons = data.GetData<Dictionary<string, string>>("StopReason");

            if (null == reasons)
            {
                reasons = new Dictionary<string, string>();
                data.SetData("StopReason", reasons);
            }
            reasons.IDictionaryTryAdd(reason, reason);
        }
        /// <summary>
        /// 返回 移除后是否仍有StopReason
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        static bool StopReason_Remove(this ApiNode data, string reason)
        {
            var reasons = data.GetData<Dictionary<string, string>>("StopReason");

            if (null == reasons) return false;

            reasons.Remove(reason);
            return reasons.Count != 0;
        }


        #endregion

        public static void StopReason_Add(this ApiNode apiNode, ApiLoadBalancingMng apiLoadBalancingMng, string reason)
        {
            apiNode.StopReason_Add(reason);
            apiNode.Stop(apiLoadBalancingMng);
        }

        public static void StopReason_Remove(this ApiNode apiNode, ApiLoadBalancingMng apiLoadBalancingMng, string reason)
        {
            if (!apiNode.StopReason_Remove(reason))
            {
                apiNode.Start(apiLoadBalancingMng);
            }
        }

    }
}
