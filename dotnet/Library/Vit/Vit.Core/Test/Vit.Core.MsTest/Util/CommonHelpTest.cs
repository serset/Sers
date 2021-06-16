using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Util.Common;

namespace Vit.Core.MsTest.Util
{
    [TestClass]
    public class CommonHelpTest
    {
        /// <summary>
        /// 动态生产有规律的ID Snowflake算法是Twitter的工程师为实现递增而不重复的ID实现的
        /// </summary>
        [TestMethod]
        public void SnowflakeTestMethod1()
        {
            var ids = new List<long>();
            for (int i = 0; i < 100; i++)//测试有序ID
            {
                ids.Add(CommonHelp.NewSnowflakeGuidLong());
            }
            for (int i = 0; i < ids.Count - 1; i++)
            {
                Assert.IsTrue(ids[i] < ids[i + 1]);
            }
        }
    }
}
