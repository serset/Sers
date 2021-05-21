using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vit.Core.Util.Shell;

namespace App
{
    class Program
    {

 
        static void Main(string[] args)
        {

            args = new[] { "cmd.exe", "", "dotnet --help" + Environment.NewLine + "exit" + Environment.NewLine };
            if (args.Length < 3) args = args.AsQueryable().Concat(new string[] { null, null, null }).ToArray();

            var fileName = args[0];
            var arguments = args[1];
            var input = args[2];
            OsShell.Shell(fileName, arguments, out var output, input: input);
            Console.WriteLine(output);
        }


        static void Main2(string[] args)
        {
            if (args.Length < 3) args = args.AsQueryable().Concat(new string[] { null, null, null }).ToArray();

            using (var shell = new ShellStream(args[0], args[1], args[2]))
            {
                var reader = shell.StandardOutput;
                var writer = shell.StandardInput;
                Task.Run(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var line = reader.ReadLine();
                            Console.WriteLine(line);
                        }
                    }
                    catch (Exception)
                    {
                    }

                });

                while (true)
                {
                    var line = Console.ReadLine();
                    writer.WriteLine(line);
                }
            }
        }




    }
}
