using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dash.Infrastructure
{
    public static class IdGenerator
    {
        public static string Generate(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

                return string.Concat(Convert.ToBase64String(hash).ToCharArray().Where(char.IsLetterOrDigit).Take(9));
            }
        }
    }
}