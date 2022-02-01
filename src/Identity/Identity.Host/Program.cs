using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MagicMedia.Identity;

public class Program
{
    public static void Main(string[] args)
    {
        //LoggingConfig.Configure("Identity");

        try
        {
            //Log.Information("Starting IdentityServer");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            //Log.Fatal(ex, "IdentityServer terminated unexpectedly");
        }
        finally
        {
            //Log.CloseAndFlush();
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
