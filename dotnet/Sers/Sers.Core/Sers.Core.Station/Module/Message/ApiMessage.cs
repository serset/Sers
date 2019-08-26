using System;
using Sers.Core.Module.Message;
using System.Collections.Generic;
using Sers.Core.Module.Rpc;
using Sers.Core.Extensions;

namespace Sers.Core.Module.Api.Message
{
    public class ApiMessage: SersFile
    {
        public ApiMessage() { }

        public ApiMessage(ArraySegment<byte> oriData):base(oriData)
        {
        }

        public override List<ArraySegment<byte>> Files
        {
            get
            {
                var files = base.Files;
                if (files == null)
                {
                    base.Files= files = new List<ArraySegment<byte>>(2) { ArraySegmentByteExtensions.Null, ArraySegmentByteExtensions.Null };
                }
                return files;
            }
        }

        /// <summary>
        /// RpcContextData Files[0]
        /// </summary>
        public ArraySegment<byte> rpcContextData_OriData
        {
            get
            {
                return Files[0];
            }
            set
            {
                Files[0] = value;
            }
        }

        public void RpcContextData_OriData_Set(IRpcContextData rpcData)
        {
            rpcContextData_OriData = rpcData.PackageOriData();
        }


        /// <summary>
        /// value Files[1]
        /// 若为Request 则为 ArgValue
        /// 若为Reply则 为 ReturnValue
        /// </summary>
        public ArraySegment<byte> value_OriData
        {
            get
            {
                return Files[1];
            }
            set
            {
                Files[1] = value;
            }
        }

    }
}
