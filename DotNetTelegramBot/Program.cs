
using DotNetTelegramBot.LoggingFile;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace DotNetTelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            // Change the minimum number of worker threads to four, but
            // keep the old setting for minimum asynchronous I/O 
            // completion threads.
            if (ThreadPool.SetMinThreads(320, minIOC))
            {
                // The minimum number of threads was set successfully.
                CreateHostBuilder(args).Build().Run();
            }
            else
            {
                // The minimum number of threads was not changed.
            }


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                configBuilder.AddJsonFile("appsettings.json", optional: true);
                configBuilder.AddJsonFile("krakensettings.json", optional: true);
                configBuilder.AddEnvironmentVariables();
            })
            .UseWindowsService(options =>
            {
                options.ServiceName = "Bitbu.ChartService";
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();
                services.AddSingleton<ILoggerManager, LoggerManager>();
                services.AddHostedService<BotService>();
                services.BuildServiceProvider();
            });
    }
}
