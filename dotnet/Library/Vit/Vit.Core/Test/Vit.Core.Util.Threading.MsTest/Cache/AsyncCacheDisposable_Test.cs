using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Util.Threading.Cache;

namespace Vit.Core.Util.Threading.MsTest.Cache
{
    [TestClass]
    public class AsyncCacheDisposable_Test : AsyncCache_BaseTest
    {

        AsyncCache<string> asyncCache = new AsyncCache<string>();

        public override void RunTest(string name)
        {
            using var scope = asyncCache.NewScope();

            //set
            asyncCache.Value = name;

            Thread.Sleep(10);

            //get
            var actual = asyncCache.Value;
            Assert.AreEqual(name, actual);
        }

        public override async Task RunTestAsync(string name)
        {
            using var scope = asyncCache.NewScope();

            //set
            await Set(name);

            await Task.Delay(8);

            //get
            var actual = await Get();

            Assert.AreEqual(name, actual);
        }

        async Task Set(string name)
        {
            asyncCache.Value = name;
            await Task.Delay(1);
        }

        async Task<string> Get()
        {
            var name = asyncCache.Value;
            await Task.Delay(1);
            return name;
        }


    }


}
