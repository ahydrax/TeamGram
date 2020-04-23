using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeamGram.Controllers
{
    [Authorize]
    public class AuthorizeController : Controller
    {
        protected bool IsAuthenticated => HttpContext.User.Identity.IsAuthenticated;

        protected string? Username => HttpContext.User.Identity.Name;
    }
}
