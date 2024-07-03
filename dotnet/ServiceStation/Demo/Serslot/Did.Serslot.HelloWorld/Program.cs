using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Vit.Extensions;   //---- add code 1

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.TryUseSerslot();  //---- add code 2 

            builder.Services.AddControllers();

            var app = builder.Build();


            //app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

    }
}
