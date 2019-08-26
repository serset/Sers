using Sers.Core.Extensions;
using Sers.ServiceStation;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {

            ServiceStation.Init();

            #region 使用扩展消息队列            
            ServiceStation.Instance.mqMng.UseZmq();
            #endregion

            ServiceStation.Discovery(typeof(Program).Assembly);

            ServiceStation.Start();

            ServiceStation.RunAwait();

        }
    }
}
