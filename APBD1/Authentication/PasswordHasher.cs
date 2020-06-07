using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace APBD1.DAL
{
    public class PasswordHasher
    {
        private readonly string _password;
        private string _salt;

        public PasswordHasher(string password)
        {
            _password = password;
        }

        public PasswordHasher(string password, string salt)
        {
            _password = password;
            _salt = salt;
        }

        public string Salt
        {
            get
            {
                return _salt ??= GetNewSalt();
            }
        }

        public string GetHashed()
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: _password,
                salt: Encoding.UTF8.GetBytes(this.Salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            );

            return Convert.ToBase64String(valueBytes);
        }

        private static string GetNewSalt()
        {
            return Guid.NewGuid().ToString();
        }
    }
}