namespace LogViewer.Models.Encrypt
{
    /// <summary>
    /// For main crypting
    /// </summary>
    public static class Md5Encrypt
    {
        /// <returns>string hash</returns>
        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
