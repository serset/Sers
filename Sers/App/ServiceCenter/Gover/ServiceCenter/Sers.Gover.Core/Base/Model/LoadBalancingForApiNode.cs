using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Log;
using Sers.ServiceCenter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sers.Gover.Core
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
        public ApiNode GetCurApiNodeBalancing()
        {
            //TODO： 负载均衡的实现
 
            int cur= Interlocked.Increment(ref curIndex); 

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
                throw;
                Logger.log.Error(ex);
            }
            return null;            
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
