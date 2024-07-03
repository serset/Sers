using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Vit.Extensions;   //---- add code 1

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region #1 config WebHost
            {
                builder.WebHost
                    .TryUseSerslot()  //---- add code 2
                    .UseUrls(Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<string[]>("server.urls"))  //---- add code 3
                    ;
            }
            #endregion


            #region ##2 Add services to the container.
            {
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                {
                    //Json Serialize config

                    options.JsonSerializerOptions.AddConverter_Newtonsoft();
                    options.JsonSerializerOptions.AddConverter_DateTime();


                    options.JsonSerializerOptions.IncludeFields = true;

                    // JsonNamingPolicy.CamelCase makes the first letter lowercase (default), null leaves case unchanged
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                    // set the JSON encoder to allow all Unicode characters, preventing the default behavior of encoding non-ASCII characters.
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);

                    // Ignore null values
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

                    // extra comma at the end of a list of JSON values in an object or array is allowed (and ignored) within the JSON payload being deserialized.
                    options.JsonSerializerOptions.AllowTrailingCommas = true;

                });

            }
            #endregion

            var app = builder.Build();


            //app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

    }
}
