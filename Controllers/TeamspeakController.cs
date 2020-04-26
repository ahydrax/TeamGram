using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TeamGram.Phrases;
using TeamGram.Teamspeak;

namespace TeamGram.Controllers
{
    [Route("teamspeak")]
    public class TeamspeakController : AuthorizeController
    {
        private readonly IMongoDatabase _database;
        [NotNull] private readonly ITeamspeakUsersProvider _teamspeakUsersProvider;

        public TeamspeakController([NotNull] IMongoDatabase database, [NotNull] ITeamspeakUsersProvider teamspeakUsersProvider)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _teamspeakUsersProvider = teamspeakUsersProvider ?? throw new ArgumentNullException(nameof(teamspeakUsersProvider));
        }

        [HttpGet("users")]
        public async Task<string[]> GetUsers(CancellationToken ct)
        {
            var users = await _teamspeakUsersProvider.GetUsers(ct);
            return users;
        }

        [HttpGet("phrases/greetings")]
        public async Task<IActionResult> GreetingsPage()
        {
            var greetings = await _database.CustomPhrases().GetAll<CustomPhrase, UserGreetingPhrase>();
            return View(greetings);
        }
        
        [HttpGet("phrases/farewells")]
        public async Task<IActionResult> FarewellsPage()
        {
            var farewells = await _database.CustomPhrases().GetAll<CustomPhrase, UserFarewellPhrase>();
            return View(farewells);
        }
        
        [HttpGet("phrases/emptyserver")]
        public async Task<IActionResult> EmptyServerPage()
        {
            var serverIsEmptyPhrases = await _database.CustomPhrases().GetAll<CustomPhrase, ServerIsEmptyPhrase>();
            return View(serverIsEmptyPhrases);
        }

        [HttpPost("greetings")]
        public async Task<IActionResult> AddGreeting(
            [FromForm(Name = "username")] string username,
            [FromForm(Name = "template")] string template)
        {
            if (username != null && template != null)
            {
                await _database.CustomPhrases().InsertOneAsync(new UserGreetingPhrase(null, username, template));
            }

            return RedirectToAction("GreetingsPage");
        }

        [HttpPost("farewell")]
        public async Task<IActionResult> AddFarewell(
            [FromForm(Name = "username")] string username,
            [FromForm(Name = "template")] string template)
        {
            if (username != null && template != null)
            {
                await _database.CustomPhrases().InsertOneAsync(new UserFarewellPhrase(null, username, template));
            }

            return RedirectToAction("FarewellsPage");
        }

        [HttpPost("emptyserver")]
        public async Task<IActionResult> AddEmptyServerMessage([FromForm(Name = "text")] string text)
        {
            if (text != null)
            {
                await _database.CustomPhrases().InsertOneAsync(new ServerIsEmptyPhrase(null, text));
            }

            return RedirectToAction("EmptyServerPage");
        }

        [HttpPost("phrase/{id}/delete")]
        public async Task<IActionResult> PhraseDelete(string id, [FromForm(Name = "pageUrl")] string pageUrl)
        {
            await _database.CustomPhrases().DeleteOneAsync(x => x.Id == id);
            return Redirect(pageUrl);
        }
    }
}
