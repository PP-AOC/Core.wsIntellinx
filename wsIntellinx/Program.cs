using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wsIntellinx
{
    /// <summary>
    /// Universal Data SearchDriverConn
    /// </summary>
    public class Program
    {
        /// <summary>
        /// main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Service terminated unexpectedly.", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// AddEnvironmentVariables is automatically called when you initialize a new WebHostBuilder with CreateDefaultBuilder. 
        /// AddJsonFile is automatically called twice when you initialize a new WebHostBuilder with CreateDefaultBuilder.
        /// appsettings.json is read 1st then appsettings.{Environment}.json
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                               .AddJsonFile("appsettings.json", false, true)
                               .Build();
            var serilogConfig = new LoggerConfiguration()
                                    .ReadFrom.Configuration(config) // <= init logger
                                    .WriteTo.Console();
            return WebHost.CreateDefaultBuilder(args)
                                           .UseStartup<Startup>()
                                           .UseSerilog()
                                           .ConfigureLogging(builder =>
                                           {
                                               builder.AddSerilog(serilogConfig.CreateLogger());
                                           });
        }
    }
}
