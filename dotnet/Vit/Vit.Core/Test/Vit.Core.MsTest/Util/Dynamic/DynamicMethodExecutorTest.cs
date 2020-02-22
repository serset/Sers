
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Util.Dynamic;
using System.Reflection;
using System.Diagnostics;
using System;

namespace Vit.Core.MsTest.Util.Dynamic
{

    [TestClass]
    public class DynamicMethodExecutorTest
    {
        public int Add(int a1, int a2) { return a1 + a2; }



        [TestMethod]
        public void Test()
        {
            //(x.1)构建环境
            object instance = this;
            MethodInfo methodInfo = this.GetType().GetMethod("Add");
            object[] parameters = new object[] { 1, 2 };


            //(x.2)编译
            DynamicMethodExecutor executor = new DynamicMethodExecutor(methodInfo);

            //(x.3)调用
            //int ret_ = (int)methodInfo.Invoke(instance, parameters);
            int ret = (int)executor.Execute(instance, parameters);

            Assert.AreEqual(ret, 3);


            {
                return;

                //耗时测试
                int times = 1000000;
                Stopwatch watch1 = new Stopwatch();


                //(x.1) 用时 1t
                watch1.Reset();
                watch1.Start();
                for (int i = 0; i < times; i++)
                {
                    ret = Add(1, 2);
                }
                watch1.Stop();
                Console.WriteLine(watch1.Elapsed + " (Directly invoke)");


                //(x.2) 大概用时 5t
                watch1.Reset();
                watch1.Start();
                for (int i = 0; i < times; i++)
                {
                    ret = (int)executor.Execute(instance, parameters);
                }
                watch1.Stop();
                Console.WriteLine(watch1.Elapsed + " (Dynamic   invoke)");

                //(x.3) 大概用时 40t
                watch1.Reset();
                watch1.Start();
                for (int i = 0; i < times; i++)
                {
                    ret = (int)methodInfo.Invoke(instance, parameters);
                }
                watch1.Stop();
                Console.WriteLine(watch1.Elapsed + " (Reflection  invoke)");

            }
      

        }

    }
}
