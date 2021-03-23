using System.Runtime.CompilerServices;
using Sers.Core.CL.MessageOrganize;
using System.Threading;
using Vit.Core.Util.Pipelines;

namespace Vit.Extensions
{
    public static partial class IOrganizeConnectionExtensions
    {

        public static AutoResetEvent curAutoResetEvent =>
           _curAutoResetEvent.Value ?? (_curAutoResetEvent.Value = new AutoResetEvent(false));

        static System.Threading.ThreadLocal<AutoResetEvent> _curAutoResetEvent = new System.Threading.ThreadLocal<AutoResetEvent>();


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
