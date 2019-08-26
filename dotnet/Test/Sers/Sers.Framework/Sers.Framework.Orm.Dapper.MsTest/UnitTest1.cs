
using Dapper.Contrib.Extensions;



using Framework.Orm.Dapper.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsTest.Model;

namespace MsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]       
        public void TestMethod1( )
        {
            using (var conn = MysqlConfig.GetOpenConnection())
            {
                var users= conn.GetAll<User>();

                Assert.AreEqual(1, 1);

            }
        }
    }
}
