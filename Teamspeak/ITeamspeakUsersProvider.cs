using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TeamGram.Teamspeak
{
    public interface ITeamspeakUsersProvider
    {
        Task<string[]> GetUsers(CancellationToken cancellationToken = default);
        Task<ServerDetailedInfo> GetDetailedInfo(CancellationToken cancellationToken = default);
    }

    public class ServerDetailedInfo
    {
        public const string QUERY_API_BOTS_GROUPNAME = "__query_bots";

        public Dictionary<string, UserDetailedInfo[]> AllUsersAndBotsOnChannels { get; set; }

        public Dictionary<string, UserDetailedInfo[]> GetChannelsWithUsers()
            => AllUsersAndBotsOnChannels
                .Where(x => x.Key != QUERY_API_BOTS_GROUPNAME)
                .ToDictionary(x => x.Key, x => x.Value);

        public UserDetailedInfo[] GetBots()
            => AllUsersAndBotsOnChannels
                .FirstOrDefault(x => x.Key == QUERY_API_BOTS_GROUPNAME)
                .Value;
    }

    public class UserDetailedInfo
    {
        public string Username { get; set; }
        public string IpAddress { get; set; }
    }
}
