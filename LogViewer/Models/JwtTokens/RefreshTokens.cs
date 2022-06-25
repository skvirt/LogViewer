using LogViewer.Models.Auth;
using LogViewer.Models.DataBase;
using LogViewer.Models.Encrypt;
using Microsoft.EntityFrameworkCore;

namespace LogViewer.Models.JwtTokens
{
    public static class RefreshTokens
    {
        /// <returns>TRUE if ok; false - account not found in database</returns>
        public static async Task<bool> UpdateRefreshToken(string login, string password)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                // load full user
                Account? account = await context.Accounts.FirstOrDefaultAsync(a => a.Login == login && a.Password == password);
                if (account is null) return false; // not found in db

                // add refresh token
                account.RefreshToken = GetNewRefreshToken(account.Login!, account.Password!);
                account.TokenExpirate = DateTimeOffset.UtcNow.AddDays(AuthOptions.REFRESH_TOKEN_VALIDITY_IN_DAYS).ToUnixTimeSeconds();

                context.Accounts.Update(account);
                await context.SaveChangesAsync();

                return true;
            }
        }
        /// <returns>TRUE if ok (token not expired)</returns>
        public static bool CheckTokenExp(long tokenExpirate)
        {
            if (tokenExpirate > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                return true;

            return false;
        }
        /// <returns>TRUE if ok</returns>
        public static bool CheckRefreshToken(string login, string password, string token)
        {
            string encryptedLogin = Md5Encrypt.CreateMD5(login);
            string encryptedPass = Md5Encrypt.CreateMD5(password);

            if (token.Contains(encryptedLogin) && token.Contains(encryptedPass))
                return true;

            return false;
        }
        public static string GetNewRefreshToken(string login, string password)
        {
            string encryptedLogin = Md5Encrypt.CreateMD5(login);
            string encryptedPass = Md5Encrypt.CreateMD5(password);

            return Guid.NewGuid().ToString() + encryptedPass + encryptedLogin;
        }

        /// <returns>TRUE if expired</returns>
        public static bool IsExpired(DateTime ValidTo)
        {
            if (ValidTo.ToUniversalTime() < DateTime.UtcNow)
                return true; // expired

            return false;
        }
    }
}
