using System;
using System.Threading;

namespace Sers.Core.Util.Threading
{
    public class SersTimer :  IDisposable
    {
        private Timer _timer=null;
        /// <summary>
        /// Seconds
        /// </summary>
        public double interval;
        public TimerCallback timerCallback;
        public void Start()
        {
            Stop();

            var time = TimeSpan.FromSeconds(interval);
            _timer = new Timer(timerCallback, null, time, time);             
        }

   

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, 0);
                _timer.Dispose();
                _timer = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
