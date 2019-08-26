using Sers.ServiceStation;

namespace Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceStation.Init(); 

            ServiceStation.Discovery(typeof(Program).Assembly);

            ServiceStation.Start();

            ServiceStation.RunAwait();

        }
    }
}
