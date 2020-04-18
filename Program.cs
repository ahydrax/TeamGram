using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;

namespace TeamGram
{
    public class Program
    {
        public static void Main(string[] args) 
            => CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseSerilog((builderContext, loggerConfiguration) =>
                        {
                            SelfLog.Enable(Console.Error);

                            var elasticsearchOptions = new ElasticsearchSinkOptions();

                            loggerConfiguration
                                .Enrich.FromLogContext()
                                .MinimumLevel.Debug().WriteTo.ColoredConsole()
                                //.MinimumLevel.Information()
                                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                //.MinimumLevel.Override("System", LogEventLevel.Warning)
                                //.WriteTo.Elasticsearch(elasticsearchOptions)
                                ;
                        })
                        .UseStartup<Startup>()
                        .SuppressStatusMessages(true);
                });
    }
}
