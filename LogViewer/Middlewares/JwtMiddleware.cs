using LogViewer.Models.Auth;
using LogViewer.Models.DataBase;
using LogViewer.Models.JwtTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogViewer.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        public JwtMiddleware(RequestDelegate requestDelegate)
        {
            next = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string? token = context.Request.Cookies["token"];

            if (!string.IsNullOrEmpty(token))
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                bool expired = RefreshTokens.IsExpired(jwt.ValidTo);

                if (expired)
                {
                    token = await UpdateTokens(jwt); // if null, http code 401 will checked by app.UseStatusCodePages

                    context.Response.Cookies.Append("token", token, AuthOptions.ACCESS_TOKEN_COOKIE_OPTIONS);
                }
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }

            await next.Invoke(context);
        }
        /// <returns>Returns string.Empty if account not found in db, refreshToken none or access token exiped too long
        /// in other cases returns accessToken</returns>
        private async Task<string> UpdateTokens(JwtSecurityToken jwt)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                List<Claim> claims = jwt.Claims.ToList();
                string accId = claims[0].Value; // Id claim

                // Load full account info with tracking
                Account? account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id.ToString() == accId);
                if (account is null) return string.Empty; // not found in db

                
                if (!string.IsNullOrEmpty(account?.RefreshToken) // if r-token exist
                    && RefreshTokens.CheckRefreshToken(account.Login!, account.Password!, account.RefreshToken) // if r-token fake
                    && RefreshTokens.CheckTokenExp(account.TokenExpirate) // if r-token expired
                    && AccessTokens.CheckAccessToken(jwt.ValidTo)) // if a-token expired too long
                {
                    // update refresh token
                    await RefreshTokens.UpdateRefreshToken(account.Login!, account.Password!);
                    
                    // update access token
                    string accessToken = await AccessTokens.TryGetToken(account.Login!, account.Password!);
                    return accessToken;
                }
                return string.Empty;
            }
        }

    }
    
}
