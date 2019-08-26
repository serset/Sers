using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sers.Apm.SkyWalking.Core.Model
{
    public class RequestModel
    {
        public string requestId;
        public string parentRequestId;

        /// <summary>
        /// 建议为 DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        /// </summary>
        public long startTime;
        /// <summary>
        /// 建议为 DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        /// </summary>
        public long endTime;


        public DateTimeOffset startTime_dt { set { startTime = value.ToUnixTimeMilliseconds(); } }
        public DateTimeOffset endTime_dt { set { endTime = value.ToUnixTimeMilliseconds(); } }


        public string route;

        public string ext;

        static DateTimeOffset DateTimeToDateTimeOffset(DateTime utcTime1)
        {
            utcTime1 = DateTime.SpecifyKind(utcTime1, DateTimeKind.Utc);
            DateTimeOffset utcTime2 = utcTime1;
            return utcTime2;
        }
        static long DateTimeToUnixTimeMilliseconds(DateTime utcTime1)
        {
            return DateTimeToDateTimeOffset(utcTime1).ToUnixTimeMilliseconds();
        }


        public static List<RequestModel> CreateDemo()
        {
            string prefix = "/q4";
            var reqs = new List<RequestModel>();

            /*
                /root                    0    -     2000
                 |                      
                 |-----  /A              10   -     1000
                 |  |
                 |  |------ /A/1         20    -     500
                 |
                 |-----  /B              1010  -     1900


            //*/

            var now = DateTime.Now.AddSeconds(-5);
           

            reqs.Add(new RequestModel
            {
                requestId = "root",
                parentRequestId = null,
                startTime_dt = now,
                endTime_dt = now.AddMilliseconds(2000),
                route = prefix+"/root"
            });

            reqs.Add(new RequestModel
            {
                requestId = "A",
                parentRequestId = "root",
                startTime_dt = now.AddMilliseconds(10),
                endTime_dt = now.AddMilliseconds(1000),
                route = prefix+"/A"
            });

            reqs.Add(new RequestModel
            {
                requestId = "A1",
                parentRequestId = "A",
                startTime_dt = now.AddMilliseconds(20),
                endTime_dt = now.AddMilliseconds(500),
                route = prefix + "/A/1"
            });

            reqs.Add(new RequestModel
            {
                requestId = "B",
                parentRequestId = "root",
                startTime_dt = now.AddMilliseconds(1010),
                endTime_dt = now.AddMilliseconds(1900),
                route = prefix + "/B"
            });

            return reqs;

        }
    }
}
