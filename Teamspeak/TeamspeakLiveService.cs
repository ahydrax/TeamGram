using System;
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
using TeamSpeak3QueryApi.Net.Specialized.Responses;

namespace TeamGram.Teamspeak
{
    public class TeamspeakLiveService : ITeamspeakUsersProvider, IHostedService
    {
        private readonly TeamspeakConfiguration _teamspeakConfiguration;
        private readonly IMediator _mediator;
        private readonly ILogger<TeamspeakLiveService> _logger;
        private readonly ConcurrentDictionary<int, string> _usernameCache;
        private Task? _keepAliveTask;
        private CancellationTokenSource? _keepAliveCancellationTokenSource;
        private readonly TeamSpeakClient _teamspeakClient;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

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
            => await DoWithClientLock(async client =>
            {
                _logger.LogInformation("Starting teamspeak client...");
                await client.Connect();
                await client.Login(_teamspeakConfiguration.Username, _teamspeakConfiguration.Key);
                _logger.LogInformation("Teamspeak bot connected");

                await client.UseServer(1);
                _logger.LogInformation("Server changed");

                var me = await client.WhoAmI();
                _logger.LogInformation($"Connected using username {me.NickName}");

                var users = await client.GetClients();
                foreach (var clientInfo in users)
                {
                    _usernameCache.TryAdd(clientInfo.Id, clientInfo.NickName);
                }

                await client.RegisterServerNotification();

                client.Subscribe<ClientEnterView>(UserJoined);
                client.Subscribe<ClientLeftView>(UserLeft);
                _logger.LogInformation("Subscribed to notifications");

                _keepAliveCancellationTokenSource = new CancellationTokenSource();
                _keepAliveTask = Task.Factory.StartNew(
                    _ => KeepAlive(_keepAliveCancellationTokenSource.Token),
                    cancellationToken,
                    TaskCreationOptions.LongRunning);
            }, cancellationToken);

        public async Task<string[]> GetUsers(CancellationToken cancellationToken = default)
            => await DoWithClientLock(async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var users = await client.GetClients();
                return users
                    .Where(x => x.Type == ClientType.FullClient)
                    .Select(x => x.NickName)
                    .OrderBy(x => x)
                    .ToArray();
            }, cancellationToken);

        public async Task<ServerDetailedInfo> GetDetailedInfo(CancellationToken cancellationToken = default)
            => await DoWithClientLock(async client =>
            {
                var clients = await client.GetClients();

                var clientInfos = new List<GetClientDetailedInfo>();
                foreach (var clientInfo in clients)
                {
                    var clientDetailedInfo = await client.GetClientInfo(clientInfo.Id);
                    clientInfos.Add(clientDetailedInfo);
                }

                var queryApiBots = clientInfos
                    .Where(clientInfo => clientInfo.Type == ClientType.Query)
                    .Select(clientInfo => new UserDetailedInfo
                    {
                        Username = clientInfo.NickName,
                        IpAddress = clientInfo.ConnectionIp
                    })
                    .ToArray();

                var channels = await client.GetChannels();

                var result = channels.Select(channelInfo =>
                    {
                        var channelClients = clientInfos
                            .Where(clientInfo => clientInfo.ChannelId == channelInfo.Id)
                            .Where(clientInfo => clientInfo.Type == ClientType.FullClient)
                            .Select(clientInfo => new UserDetailedInfo
                            {
                                Username = clientInfo.NickName,
                                IpAddress = clientInfo.ConnectionIp
                            })
                            .ToArray();
                        return (channelInfo.Name, channelClients);
                    })
                    .Append((ServerDetailedInfo.QUERY_API_BOTS_GROUPNAME, queryApiBots))
                    .ToArray();

                return new ServerDetailedInfo
                {
                    AllUsersAndBotsOnChannels = result.ToDictionary(x => x.Item1, x => x.Item2)
                };
            }, cancellationToken);

        private void UserJoined(IReadOnlyCollection<ClientEnterView> view)
        {
            foreach (var clientEnterView in view)
            {
                if (clientEnterView.Type != ClientType.FullClient) continue;

                var username = clientEnterView.NickName;
                _logger.LogInformation("User joined {username}", username);
                _usernameCache.TryAdd(clientEnterView.Id, username);
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

        private async Task KeepAlive(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                await DoWithClientLock(async client => { await client.WhoAmI(); }, cancellationToken);
                _logger.LogInformation("Keep alive sent");
            }
        }

        private async Task DoWithClientLock(
            Func<TeamSpeakClient, Task> action,
            CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                await action(_teamspeakClient);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<T> DoWithClientLock<T>(
            Func<TeamSpeakClient, Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                return await action(_teamspeakClient);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
            => await DoWithClientLock(async client =>
            {
                _logger.LogInformation("Client disposal initiated");

                if (_keepAliveTask != null && _keepAliveCancellationTokenSource != null)
                {
                    _keepAliveCancellationTokenSource.Cancel();
                    try
                    {
                        await _keepAliveTask;
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation("Keep alive stopped");
                    }
                }

                client.Unsubscribe<ClientEnterView>();
                client.Unsubscribe<ClientLeftView>();
                client.Dispose();
                _logger.LogInformation("Client disposed");
            }, cancellationToken);
    }
}
