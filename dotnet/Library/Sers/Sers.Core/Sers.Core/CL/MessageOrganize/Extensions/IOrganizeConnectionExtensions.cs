using System.Runtime.CompilerServices;
using Sers.Core.CL.MessageOrganize;
using System.Threading;
using Vit.Core.Util.Pipelines;
using System;

namespace Vit.Extensions
{
    public static partial class IOrganizeConnectionExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetConnKey(this IOrganizeConnection conn)
        {
            return conn.GetHashCode();
        }



        #region static curAutoResetEvent
        public static AutoResetEvent curAutoResetEvent =>
            _curAutoResetEvent ?? (_curAutoResetEvent = new AutoResetEvent(false));

        [ThreadStatic]
        static AutoResetEvent _curAutoResetEvent;
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SendRequest(this IOrganizeConnection conn, Vit.Core.Util.Pipelines.ByteData requestData, out ByteData replyData, int requestTimeoutMs = 60000)
        {
            ByteData _replyData = null;

            AutoResetEvent mEvent = curAutoResetEvent;
            mEvent.Reset();

            conn.SendRequestAsync(null, requestData, (sender, replyData_) => {
                _replyData = replyData_;
                mEvent?.Set();
            });

            bool success;
            try
            {
                success = mEvent.WaitOne(requestTimeoutMs);
            }
            finally
            {
                mEvent = null;
            }


            if (success)
            {
                replyData = _replyData;
                return true;
            }
            else
            {                
                replyData = null;
                return false;
            }
        }      
    
 
    }
}
