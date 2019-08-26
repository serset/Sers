using Sers.Framework.DynamicCompile;
using System;

namespace ConsoleApp1
{
    public class ModelA
    {
        public int value;
        public int result;
    }

    class Program
    {

        static void Main(string[] args)
        {
            #region demo1
            try
            {
                string testClass = @" 
Action<ConsoleApp1.ModelA> action=(m)=>{ m.result = m.value + (int)args[0]; };
return action;
";
                var action = CSharpCompiler.RunCodeBlock(new[] { "Sers.Framework.DynamicCompile.Demo.dll" }, testClass,3) as Action<ConsoleApp1.ModelA>;

                var m = new ModelA { value = 121 };
                if (action != null)
                {
                    action(m);
                }
            }
            catch (Exception ex)
            {
                ex = ex.GetBaseException();
            }
            #endregion


            #region demo2
            try
            {
                string testClass = @"using System; 
              namespace test{
               public class tes
               {
                    public static Action<ConsoleApp1.ModelA> getAction()
                    {
                        return (m)=>{ m.result = m.value + 1; };
                    }

               }
              }";


                var assembly = CSharpCompiler.CompileToAssembly(new[] { "Sers.Framework.DynamicCompile.Demo.dll" }, testClass);

                var type = assembly.GetType("test.tes");

                var method = type?.GetMethod("getAction");
                var action = method?.Invoke(null, null) as Action<ConsoleApp1.ModelA>;

                var m = new ModelA { value = 121 };
                if (action != null)
                {
                    action(m);
                }
            }
            catch (Exception ex)
            {
                ex = ex.GetBaseException();
            }
            #endregion

        }




    }
}
