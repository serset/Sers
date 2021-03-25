using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Sers.Core.Module.Api.ApiDesc;
using Sers.ServiceCenter.Entity;
using Vit.Core.Module.Log;

namespace Sers.Gover.Base.Model
{
    public class LoadBalancingForApiNode
    {

        /// <summary>
        /// TODO:待完善
        /// </summary>
        public SsApiDesc apiDesc => apiNodes[0]?.apiDesc;

        List<ApiNode> apiNodes = new List<ApiNode>();


        private int _apiNodeCount;

        public int apiNodeCount => _apiNodeCount;

       

        int curIndex = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiNode GetCurApiNodeBalancing()
        {
            //TODO： 负载均衡的实现

            int cur = Interlocked.Increment(ref curIndex);

            try
            {
                if (cur >= (int.MaxValue - 10000))
                {
                    cur = curIndex = 0;
                }
                else
                {
                    cur %= _apiNodeCount;
                }
                return apiNodes[cur];
            }
            catch (Exception ex)
            {
                //TODO: not expected               
                Logger.Error(ex);
                throw;
            }
        }


        public void AddApiNode(ApiNode apiNode)
        {
            apiNodes.Insert(0, apiNode);
            _apiNodeCount = apiNodes.Count;
        }

        public void RemoveApiNode(ApiNode apiNode)
        {
            apiNodes.Remove(apiNode);
            _apiNodeCount = apiNodes.Count;
        }

    }
}
