using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamGram.Teamspeak;

namespace TeamGram.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITeamspeakUsersProvider _teamspeakUsersProvider;

        public HomeController(ITeamspeakUsersProvider teamspeakUsersProvider)
        {
            _teamspeakUsersProvider = teamspeakUsersProvider;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _teamspeakUsersProvider.GetDetailedInfo(cancellationToken);
            return View(users);
        }
    }
}
