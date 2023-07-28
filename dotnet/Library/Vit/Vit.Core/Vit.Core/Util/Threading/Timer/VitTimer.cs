using System;
using System.Threading;

namespace Vit.Core.Util.Threading.Timer
{
    public class VitTimer : IDisposable
    {
        protected System.Threading.Timer _timer = null;

        public bool isRunning => _timer != null;

        /// <summary>
        /// 定时器间隔，单位：ms
        /// </summary>
        public virtual int intervalMs { get; set; }

        public virtual TimerCallback timerCallback { get; set; }

        public virtual void Start()
        {
            Stop();

            var time = TimeSpan.FromMilliseconds(intervalMs);
            _timer = new System.Threading.Timer(timerCallback, null, time, time);
        }



        public virtual void Stop()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, 0);
                _timer.Dispose();
                _timer = null;
            }
        }

        public virtual void Dispose()
        {
            Stop();
        }
    }
}
