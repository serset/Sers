using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sers.Core.Db.Redis.Test
{
    public class RedisQpsTest
    {
        long requestCount = 0;
        public int threadCount = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("threadCount");

        public RedisQpsTest()
        {
            requestCount = 0;
        }


        class ModelA {
            public string va;
        }

        string[] keyM = { "dbA", "model" };
        public void Start()
        {

            #region (x.1) set value to redis
            using (var db = new DbRedis())
            {
                var maA = new ModelA { va = "asddas" };
                db.Set(maA, TimeSpan.FromSeconds(600),keyM);               
            }
            #endregion

            for (int t = 0; t < threadCount; t++) {
                Task.Run((Action)ThreadReadRedis);
            }

            ThreadPrintQps();
            //Task.Run((Action)ThreadPrintQps);
        }

        void ThreadReadRedis()
        {
            using (var db = new DbRedis())
            {
                ModelA obj;
                while (true) {
                    obj=db.Get<ModelA>(keyM);
                    Interlocked.Increment(ref requestCount);
                }

            }
        }

        void ThreadPrintQps()
        {
            DateTime dtLast = DateTime.Now;
            long lastRequest = requestCount;

            DateTime dtNow;
            long curRequest;
            while (true)
            {
               
                Thread.Sleep(1000);

                dtNow = DateTime.Now;
                curRequest = requestCount;

                int qps =(int)(  (curRequest - lastRequest) / (dtNow - dtLast).TotalSeconds);
                Console.WriteLine("qps:" + qps);

                dtLast = dtNow;
                lastRequest = curRequest;

            }

        }


    }
}
