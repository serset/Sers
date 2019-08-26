using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            var time = TimeSpan.FromSeconds(interval);
            _timer = new Timer(timerCallback, null, time, time);             
        }

   

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);            
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
