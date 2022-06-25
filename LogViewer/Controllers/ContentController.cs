using LogViewer.Models;
using LogViewer.Models.DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogViewer.Controllers
{
    public class ContentController : Controller
    {
        private readonly ApplicationContext context;
        public ContentController(ApplicationContext db)
        {
            context = db;
        }
        [Authorize, HttpGet, Route("")]
        public IActionResult Logs() => View();

        [Authorize, HttpGet, Route("getlog")]
        public async Task<IActionResult> GetLog(LogFinderModel logFinderModel)
        {
            int? uid = logFinderModel.UserID;

            string date;
            if (logFinderModel.Date is not null)
            {
                DateTime dt = (DateTime)logFinderModel.Date;
                date = dt.ToShortDateString();
            }
            else 
                date = DateTime.UtcNow.ToShortDateString();

            string? type = LogTypes.logTypesKeyEnum.GetValueOrDefault(logFinderModel.Type);
            if (type == "Не выбрано") type = null;

            List<Log> logList = new List<Log>();

            if (uid is not null && type is not null)
            {
                logList = await context.LogStrings.AsNoTracking()
                    .Where(l => l.UserId == uid && l.Type == type && l.CreatedAtDateString == date)
                    .OrderByDescending(l => l.CreatedAt).ToListAsync();
            }
            else if (uid is not null)
            {
                logList = await context.LogStrings.AsNoTracking()
                    .Where(l => l.UserId == uid && l.CreatedAtDateString == date)
                    .OrderByDescending(l => l.CreatedAt).ToListAsync();
            }
            else if (type is not null)
            {
                logList = await context.LogStrings.AsNoTracking()
                    .Where(l => l.Type == type && l.CreatedAtDateString == date)
                    .OrderByDescending(l => l.CreatedAt).ToListAsync();
            }
            else
            {
                logList = await context.LogStrings.AsNoTracking()
                    .Where(l => l.CreatedAtDateString == date)
                    .OrderByDescending(l => l.CreatedAt).ToListAsync();
            }

            ViewBag.LogView = logList;
            return View("Logs");
        }
        [Authorize, HttpGet, Route("getinfo")]
        public async Task<IActionResult> GetAccountInfo()
        {
            string token = HttpContext.Request.Cookies["token"]!;
            JwtSecurityToken jwt = new JwtSecurityToken(token);
            List<Claim> claims = jwt.Claims.ToList();
            string accountId = claims[0].Value; // Id claim

            Account? account = await context.Accounts.FirstOrDefaultAsync(a => a.Id.ToString() == accountId);
            if (account is null) return BadRequest();


            return Content($"id {account.Id} / login {account.Login} / pass {account.Password} / last-ip {account.LastIp} <br />" +
                $"rtoken {account.RefreshToken} / exptoken {account.TokenExpirate}", "text/html", Encoding.UTF8);
        }
    }
}
