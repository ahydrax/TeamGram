using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using TeamGram.Configuration;
using TeamGram.Phrases;
using TeamGram.Teamspeak;
using TeamGram.Telegram;

namespace TeamGram
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ApplicationConfiguration = Configuration.Get<ApplicationConfiguration>();
        }

        private IConfiguration Configuration { get; }

        private ApplicationConfiguration ApplicationConfiguration { get; }

        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMediatR(typeof(Program).Assembly);
            services.AddSingleton(ApplicationConfiguration.Teamspeak);
            services.AddSingleton(ApplicationConfiguration.Telegram);
            services.AddSingleton(ApplicationConfiguration.Mongo);
            services.AddSingleton<IMongoClient>(x =>
            {
                var options = x.GetRequiredService<MongoConfiguration>();
                var mongoUrl = new MongoUrl(options.Uri);
                return new MongoClient(mongoUrl);
            });
            services.AddSingleton<IMongoDatabase>(x =>
            {
                var options = x.GetRequiredService<MongoConfiguration>();
                var client = x.GetRequiredService<IMongoClient>();
                return client.GetDatabase(options.Database);
            });

            services.AddSingleton<PhrasesProvider>();

            services.AddSingleton<TelegramMessagingService>();
            services.AddSingleton<IHostedService>(x => x.GetRequiredService<TelegramMessagingService>());
            services.AddSingleton<ITelegramMessageSender>(x => x.GetRequiredService<TelegramMessagingService>());

            services.AddSingleton<TeamspeakLiveService>();
            services.AddSingleton<IHostedService>(x => x.GetRequiredService<TeamspeakLiveService>());
            services.AddSingleton<ITeamspeakUsersProvider>(x => x.GetRequiredService<TeamspeakLiveService>());
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
