using LogViewer.Models;
using LogViewer.Models.Auth;
using LogViewer.Models.Encrypt;
using LogViewer.Models.DataBase;
using LogViewer.Models.JwtTokens;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LogViewer.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationContext context;
        public AuthController(ApplicationContext db)
        {
            context = db;
        }
        [HttpGet, Route("login")]
        public IActionResult Login() => View();

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (!ModelState.IsValid)
                return Content("Длина логина от 2 до 20 символов, пароля от 6 до 25");

            string hashedPassword = Md5Encrypt.CreateMD5(password);

            string token = await AccessTokens.TryGetToken(login, hashedPassword); // request access token
            if (!string.IsNullOrEmpty(token))
            {
                HttpContext.Response.Cookies.Append("token", token, AuthOptions.ACCESS_TOKEN_COOKIE_OPTIONS);
                await RefreshTokens.UpdateRefreshToken(login, hashedPassword);
                return RedirectToActionPermanent("Logs", "Content");
            }
            return NotFound("Your account is not exist on this service.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}