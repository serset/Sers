using System.Threading;
using Newtonsoft.Json;

namespace Sers.Core.Module.Counter
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Counter
    {

        public void CopyDataFrom(Counter counter)
        {
            this.sumCount = counter.sumCount;
            this.errorCount = counter.errorCount;
        }

        /// <summary>
        /// 计数时 向上级报告
        /// </summary>
        /// <param name="parentCounter"></param>
        public void ReportTo(Counter parentCounter)
        {
            this.parentCounter = parentCounter;
        }

        Counter parentCounter;
        /// <summary>
        /// 已经调用次数
        /// </summary>        
        [JsonProperty]
        public int sumCount = 0;

        /// <summary>
        /// 失败调用次数
        /// </summary>        
        [JsonProperty]
        public int errorCount = 0;
 
        public void Increment(bool success)
        {
            Interlocked.Increment(ref sumCount);
            if(!success) Interlocked.Increment(ref errorCount);

            parentCounter?.Increment(success);
        }

    }
}
