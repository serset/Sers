using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vit.Core.Util.Threading.MsTest.Cache
{
    [TestClass]
    public class AsyncLocal_Test : AsyncCache_BaseTest
    {

        readonly System.Threading.AsyncLocal<string> AsyncLocal = new System.Threading.AsyncLocal<string>();

        public override void RunTest(string name)
        {
            //set
            AsyncLocal.Value = name;

            Thread.Sleep(10);

            //get
            var actual = AsyncLocal.Value;
            Assert.AreEqual(name, actual);
        }

        public override async Task RunTestAsync(string name)
        {
            //set
            //await Set(name);
            AsyncLocal.Value = name;

            await Task.Delay(8);

            //get
            var actual = await Get();

            Assert.AreEqual(name, actual);
        }

        async Task Set(string name)
        {
            AsyncLocal.Value = name;
            await Task.Delay(1);
        }

        async Task<string> Get()
        {
            var name = AsyncLocal.Value;
            await Task.Delay(1);
            return name;
        }


    }


}
