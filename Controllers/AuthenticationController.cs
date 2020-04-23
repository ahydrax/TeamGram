using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace TeamGram.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet]
        [Route("/login")]
        public IActionResult LoginPage() => View();

        [HttpPost]
        [Route("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm(Name = "password")] string password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "root")
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginPage");
        }
    }
}
