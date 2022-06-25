using LogViewer.Models.Auth;
using LogViewer.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LogViewer.Middlewares
{
    public class IpMiddleware
    {
        private readonly RequestDelegate next;
        public IpMiddleware(RequestDelegate requestDelegate)
        {
            next = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Cookies["token"]; // get access token from cookie

            if (!string.IsNullOrEmpty(token))
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                List<Claim> claims = jwt.Claims.ToList();
                string accountId = claims[0].Value; // Id claim

                string? ip = context.Connection.RemoteIpAddress?.ToString();

                Account account = await GetSimpleModelAccountAsync(accountId);

                if(!string.IsNullOrEmpty(account.LastIp) && !string.IsNullOrEmpty(ip))
                {
                    bool result = CompareIps(ip, account.LastIp);

                    if (!result)
                    {
                        // unauthorize user
                        context.Response.Cookies.Append("token", string.Empty, AuthOptions.ACCESS_TOKEN_COOKIE_OPTIONS);

                        await UpdateLastIp(accountId, ip);
                    }
                        
                }
                else if (string.IsNullOrEmpty(ip))
                {
                    throw new Exception("Middlewares/_/InvokeAsync() string ip is null");
                }
                else if(string.IsNullOrEmpty(account.LastIp))
                {
                    await UpdateLastIp(accountId, ip);
                }
            }

            await next.Invoke(context);
        }
        private async Task UpdateLastIp(string accountId, string newIp)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Account? account = await db.Accounts.FirstOrDefaultAsync(a => a.Id.ToString() == accountId);
                if (account is null)
                    throw new Exception("Middlewares/_/UpdateLastIp() download 'null' from db");

                account.LastIp = newIp; // new last ip

                db.Accounts.Update(account);
                await db.SaveChangesAsync();
            }
        }
        private async Task<Account> GetSimpleModelAccountAsync(string accountId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Account? account = await db.Accounts.AsNoTracking()
                    .Select(a => new Account() { Id = a.Id, LastIp = a.LastIp })
                    .FirstOrDefaultAsync(a => a.Id.ToString() == accountId);

                if (account is null) 
                    throw new Exception("Middlewares/_/GetSimpleModelAccount() returns null");

                return account;
            }
        }
        /// <returns>TRUE if ok (IPs are the same)</returns>
        private bool CompareIps(string now, string last) // hash working
        {
            if(now != last)
                return false;

            return true;
        }
    }
}
