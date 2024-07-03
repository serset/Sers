using System.Threading;

namespace Vit.Core.Util.Threading.Timer
{
    /// <summary>
    /// 若前序timer回调没有执行结束，则后续回调不会被调用
    /// </summary>
    public class VitTimer_SingleThread : VitTimer
    {
        public VitTimer_SingleThread()
        {
            _timerCallback = TimerCallback;
        }

        protected int locked = 0;
        protected void TimerCallback(object state)
        {
            if (0 != Interlocked.CompareExchange(ref locked, 1, 0))
                return;

            try
            {
                _oriCallback(state);
            }
            finally
            {
                locked = 0;
            }
        }

        protected TimerCallback _oriCallback;
        protected readonly TimerCallback _timerCallback;

        public override TimerCallback timerCallback
        {
            get => _timerCallback;
            set
            {
                _oriCallback = value;
            }
        }

    }
}
