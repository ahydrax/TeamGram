using System.Threading;
using System.Threading.Tasks;

namespace TeamGram.Teamspeak
{
    public interface ITeamspeakUsersProvider
    {
        Task<string[]> GetUsers(CancellationToken cancellationToken = default);
    }
}
