using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Vit.Core.Util.Threading.Cache;

namespace Vit.Core.Util.Threading.MsTest.Cache
{
    [TestClass]
    public class AsyncCacheScope_Test : AsyncCache_BaseTest
    {
        class CacheScope : AsyncCacheScope<CacheScope>
        {
            public string name;
        }

        public override void RunTest(string name)
        {
            using var scope = new CacheScope();

            //set
            CacheScope.Instance.name = name;

            Thread.Sleep(10);

            //get
            var actual = CacheScope.Instance.name;
            Assert.AreEqual(name, actual);
        }

        public override async Task RunTestAsync(string name)
        {
            using var scope = new CacheScope();

            //set
            await Set(name);

            await Task.Delay(8);

            //get
            var actual = await Get();

            Assert.AreEqual(name, actual);
        }

        async Task Set(string name)
        {
            CacheScope.Instance.name = name;
            await Task.Delay(1);
        }

        async Task<string> Get()
        {
            var name = CacheScope.Instance.name;
            await Task.Delay(1);
            return name;
        }


    }

     
}
