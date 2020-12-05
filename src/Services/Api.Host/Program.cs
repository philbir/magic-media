using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MagicMedia.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggingConfig.Configure("Api");

            try
            {
                Log.Information("Starting Api");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "API Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(builder =>
               {
                   builder.AddJsonFile("appsettings.json");
                   builder.AddUserSecrets<Program>(optional: true);
                   builder.AddJsonFile("appsettings.local.json", optional: true);
                   builder.AddEnvironmentVariables();
               })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
