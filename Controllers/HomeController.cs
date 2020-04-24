using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamGram.Teamspeak;

namespace TeamGram.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITeamspeakUsersProvider _teamspeakUsersProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITeamspeakUsersProvider teamspeakUsersProvider, ILogger<HomeController> logger)
        {
            _teamspeakUsersProvider = teamspeakUsersProvider;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _teamspeakUsersProvider.GetDetailedInfo(cancellationToken);
            return View(users);
        }
    }
}
