using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sers.Apm.SkyWalking.Core.Model
{
    public class RequestTree : RequestModel
    {
        public RequestTree(RequestModel info)
        {
            this.requestId = info.requestId;
            this.parentRequestId = info.parentRequestId;
            this.startTime = info.startTime;
            this.endTime = info.endTime;
            this.route = info.route;           
        }

        public List<RequestTree> children;



        public static RequestTree LoadFromRequestModel(List<RequestModel> reqs)
        {
            var root = (from item in reqs where string.IsNullOrWhiteSpace(item.parentRequestId) select new RequestTree(item)).FirstOrDefault();
            if (null == root) return null;

            root.children= LoadChildren(reqs, root.requestId);
            return root;
        }

        static List<RequestTree> LoadChildren(List<RequestModel> reqs, string parentRequestId)
        {
            var children = (from item in reqs where parentRequestId==item.parentRequestId select new RequestTree(item)).ToList();
            if (children.Count == 0) return null;

            foreach (var item in children)
            {
                item.children = LoadChildren(reqs, item.requestId);
            }

            return children;
        }


    }
}
