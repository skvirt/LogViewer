using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LogViewer.Models.Auth
{
    public static class AuthOptions
    {
        public const string ISSUER = "LogViewer";
        public const string AUDIENCE = "LogViewer_Client";

        public const int ACCESS_TOKEN_VALIDITY_IN_MIN = 1;
        public const int ACCESS_TOKEN_CRITICAL_TIME_IN_MIN = 20; // time after access token expired, when user have to authorize again
        public const int REFRESH_TOKEN_VALIDITY_IN_DAYS = 1;

        public static readonly CookieOptions ACCESS_TOKEN_COOKIE_OPTIONS = new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddHours(1),
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            IsEssential = true 
        };

        private const string KEY = "MbOBEYSLnKwGxWT2";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
