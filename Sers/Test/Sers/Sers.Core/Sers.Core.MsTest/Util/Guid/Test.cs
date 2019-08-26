using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sers.Core.Util.Common;

namespace Sers.Core.MsTest.Util.Guid
{
    [TestClass]
    public class Test
    {
        /// <summary>
        /// 动态生产有规律的ID Snowflake算法是Twitter的工程师为实现递增而不重复的ID实现的
        /// </summary>
        [TestMethod]
        public void SnowflakeTestMethod1()
        {
            var ids = new List<long>();
            for (int i = 0; i < 1000000; i++)//测试同时100W有序ID
            {
                ids.Add(CommonHelp.NewGuidLong());
            }
            for (int i = 0; i < ids.Count - 1; i++)
            {
                Assert.IsTrue(ids[i] < ids[i + 1]);
            }
        }
    }
}
