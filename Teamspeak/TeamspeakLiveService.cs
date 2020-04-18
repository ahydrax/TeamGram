using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamGram.Configuration;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Notifications;

namespace TeamGram.Teamspeak
{
    public class TeamspeakLiveService : ITeamspeakUsersProvider, IHostedService
    {
        private readonly TeamspeakConfiguration _teamspeakConfiguration;
        private readonly IMediator _mediator;
        private readonly ILogger<TeamspeakLiveService> _logger;
        private readonly TeamSpeakClient _teamspeakClient;
        private readonly ConcurrentDictionary<int, string> _usernameCache;

        public TeamspeakLiveService(TeamspeakConfiguration teamspeakConfiguration,
            IMediator mediator,
            ILogger<TeamspeakLiveService> logger)
        {
            _teamspeakConfiguration = teamspeakConfiguration;
            _mediator = mediator;
            _logger = logger;
            _teamspeakClient = new TeamSpeakClient(teamspeakConfiguration.Host, teamspeakConfiguration.Port);
            _usernameCache = new ConcurrentDictionary<int, string>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting teamspeak client...");
            await _teamspeakClient.Connect();
            await _teamspeakClient.Login(_teamspeakConfiguration.Username, _teamspeakConfiguration.Key);
            _logger.LogInformation("Teamspeak bot connected");

            await _teamspeakClient.UseServer(1);
            _logger.LogInformation("Server changed");

            var me = await _teamspeakClient.WhoAmI();
            _logger.LogInformation($"Connected using username {me.NickName}");

            var users = await _teamspeakClient.GetClients();
            foreach (var clientInfo in users)
            {
                _usernameCache.TryAdd(clientInfo.Id, clientInfo.NickName);
            }

            await _teamspeakClient.RegisterServerNotification();

            _teamspeakClient.Subscribe<ClientEnterView>(UserJoined);
            _teamspeakClient.Subscribe<ClientLeftView>(UserLeft);
            _logger.LogInformation("Subscribed to notifications");
        }

        public async Task<string[]> GetUsers(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var users = await _teamspeakClient.GetClients();
            return users
                .Where(x => x.Type == ClientType.FullClient)
                .Select(x => x.NickName)
                .OrderBy(x => x)
                .ToArray();
        }

        private void UserJoined(IReadOnlyCollection<ClientEnterView> view)
        {
            foreach (var clientEnterView in view)
            {
                var username = clientEnterView.NickName;
                _logger.LogInformation("User joined {username}", username);
                _mediator.Publish(new UserJoined(username));
            }
        }

        private void UserLeft(IReadOnlyCollection<ClientLeftView> view)
        {
            foreach (var clientLeftView in view)
            {
                if (_usernameCache.TryGetValue(clientLeftView.Id, out var username))
                {
                    _logger.LogInformation("User left {username}", username);
                    _mediator.Publish(new UserLeft(username));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Client disposal initiated");
            _teamspeakClient.Unsubscribe<ClientEnterView>();
            _teamspeakClient.Unsubscribe<ClientLeftView>();
            _teamspeakClient.Dispose();
            _logger.LogInformation("Client disposed");
            return Task.CompletedTask;
        }
    }
}
