using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dotnet
{
    public class MainFinder
    {

        public static void Dotnet()
        {
            var path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            path = path.Substring(0, path.Length - 3) + "dll";

            Console.WriteLine("加载程序集：" + path);
     
            var assembly = Assembly.LoadFrom(path);
            FindAndCallMain(assembly);
        }

        public static void FindAndCallMain(Assembly assembly)
        {
            Console.WriteLine("查找Main函数");
            var main = FindMain(assembly);
            if (main == null)
            {
                Console.WriteLine("未查找到Main函数");
                return;
            }
            Console.WriteLine("调用Main函数");
            main.Invoke();
        }

        public static Action FindMain(Assembly assembly)
        {
            Action action=null;
            var method = GetMainMethod(assembly);

            if (null != method)
            {
                action = () =>
                {

                    var param = new object[method.GetParameters().Length];
                    method.Invoke(null, param);
                };
            }
             
            return action;
        }
        private static MethodInfo GetMainMethod(Assembly assembly)
        {
          
            var types = assembly?.GetTypes();

            foreach (var type in types)
            {
                var method = type.GetMethod("Main", BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
                if (null != method && method.IsStatic) return method;
            }
            return null;
        }

    }
}
