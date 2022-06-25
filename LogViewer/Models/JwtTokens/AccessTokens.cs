using LogViewer.Models.Auth;
using LogViewer.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LogViewer.Models.JwtTokens
{
    public static class AccessTokens
    {
        /// <returns>if ok returns string token; If account not found in database returns string.Empty</returns>
        public static async Task<string> TryGetToken(string login, string password)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                // load full user
                Account? account = await context.Accounts.FirstOrDefaultAsync(a => a.Login == login && a.Password == password);
                if (account is null) return string.Empty; // not found in db

                // claims
                List<Claim> claims = new() { new Claim("Id", account.Id.ToString()) };

                // create access token
                JwtSecurityToken jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.ACCESS_TOKEN_VALIDITY_IN_MIN)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512Signature)); // .HmacSha256

                string token = new JwtSecurityTokenHandler().WriteToken(jwt);
                 
                return token;
            }
        }
        /// <returns>TRUE if ok</returns>
        public static bool CheckAccessToken(DateTime jwtValidTo)
        {
            DateTime validTime = jwtValidTo.ToUniversalTime();
            DateTime nowTime = DateTime.UtcNow;

            TimeSpan resultTime = nowTime - validTime;

            if (resultTime > TimeSpan.FromMinutes(AuthOptions.ACCESS_TOKEN_CRITICAL_TIME_IN_MIN))
                return false;

            return true;
        }
    }
}
