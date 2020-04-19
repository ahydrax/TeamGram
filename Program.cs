using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using TeamGram.Configuration;

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

                            var appConfig = builderContext.Configuration.Get<ApplicationConfiguration>();
                            var esConfig = appConfig.Elastic;

                            loggerConfiguration
                                .Enrich.FromLogContext()
                                .MinimumLevel.Debug().WriteTo.ColoredConsole()
                                .MinimumLevel.Information()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                .MinimumLevel.Override("System", LogEventLevel.Warning)
                                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esConfig!.Uri!))
                                {
                                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                    AutoRegisterTemplate = true,
                                    IndexFormat = "teamgram-{0:yyyy-MM-dd}",
                                    DeadLetterIndexName = "failed-logs",
                                    TemplateName = "teamgram",
                                    ModifyConnectionSettings = x =>
                                    {
                                        return x.BasicAuthentication(esConfig.Username, esConfig.Password)
                                            .ServerCertificateValidationCallback((a, b, c, d) => true);
                                    },
                                    EmitEventFailure = EmitEventFailureHandling.WriteToFailureSink
                                });
                        })
                        .UseStartup<Startup>()
                        .SuppressStatusMessages(true);
                });
    }
}
