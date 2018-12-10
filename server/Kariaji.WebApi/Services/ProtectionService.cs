using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kariaji.WebApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace Kariaji.WebApi.Services
{
    public class ProtectionService
    {
        private const string keyForData = "data";
        private const string keyForReversiblePasswords = "passwords";

        private IDataProtectionProvider dataProtectionProvider;

        public ProtectionService(IDataProtectionProvider dataProtectionProvider)
        {
            this.dataProtectionProvider = dataProtectionProvider;
        }


        private const string passwordSaltStr = "absyak2298jsahd";
        private readonly byte[] passwordSalt = Encoding.UTF8.GetBytes(passwordSaltStr);

        public string ProtectPassword(string password)
        {
            var protector = this.dataProtectionProvider.CreateProtector(keyForReversiblePasswords);
            return Convert.ToBase64String(protector.Protect(Encoding.UTF8.GetBytes(password)));
        }

        public string UnprotectPassword(string protectedPassword)
        {
            var protector = this.dataProtectionProvider.CreateProtector(keyForReversiblePasswords);
            return Encoding.UTF8.GetString(protector.Unprotect(Convert.FromBase64String(protectedPassword)));
        }

        public string HashPassword(string password)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: passwordSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256/8));
            return hashed;
        }

        public bool TestPassword(string password, string hash) => HashPassword(password) == hash;

        public string ProtecData(object value)
        {
            var protector = this.dataProtectionProvider.CreateProtector(keyForData);
            var tokenData = JsonConvert.SerializeObject(value);
            var token = Convert.ToBase64String(protector.Protect(Encoding.UTF8.GetBytes(tokenData)));
            return token;
        }

        public T UnprotectData<T>(string token)
        {
            var protector = this.dataProtectionProvider.CreateProtector(keyForData);
            var data = protector.Unprotect(Convert.FromBase64String(token));
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));

        }


        private static string _JwtKey;
        public static string JwtKey 
        {
            get
            {
                if (_JwtKey == null)
                {
                    var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
                    byte[] randomBytes = new byte[128];
                    rngCryptoServiceProvider.GetBytes(randomBytes);
                    _JwtKey = Convert.ToBase64String(randomBytes);
                }

                return _JwtKey;
            }
        }

        private static string _Salt;
        public static string Salt
        {
            get
            {
                if (_Salt == null)
                {
                    var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
                    byte[] randomBytes = new byte[16];
                    rngCryptoServiceProvider.GetBytes(randomBytes);
                    _Salt = Convert.ToBase64String(randomBytes);
                }

                return _Salt;
            }
        }
    }

}
