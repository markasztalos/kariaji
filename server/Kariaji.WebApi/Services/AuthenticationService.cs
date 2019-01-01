using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using Newtonsoft.Json;

namespace Kariaji.WebApi.Services
{
    public class AuthenticationService
    {
        private readonly KariajiContext ctx;
        private readonly MailingService mailingSvc;
        private readonly ProtectionService protectionService;
        private readonly ConfigurationProviderService configSvc;


        public AuthenticationService(KariajiContext ctx, ProtectionService protectionService, MailingService mailingSvc, ConfigurationProviderService configSvc)
        {
            this.ctx = ctx;
            this.protectionService = protectionService;
            this.mailingSvc = mailingSvc;
            this.configSvc = configSvc;
        }
        public async Task CheckPassword(string email, string password)
        {
            var em = email.Trim().ToLower();
            var user = await this.ctx.Users.FirstOrDefaultAsync(u => u.Email == em);
            if (user == null)
            {
                throw KariajiException.NewPublic("Ez az email cím még nincs regisztrálva");
            }

            if (this.protectionService.HashPassword(password) != user.Password)
            {
                throw KariajiException.NewPublic("Hibás jelszó");
            }
        }


        public async Task<User> ConfirmRegistration(string token, string password)
        {

            var registrationData = this.protectionService.UnprotectData<RegisterTokenModel>(token);
            var date = registrationData.RequestDate;
            if (date.AddDays(1) < DateTime.Now)
            {
                throw KariajiException.NewPublic("Ez a kód már lejárt");
            }
            var email = registrationData.Email;
            var existingUser = await ctx.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (existingUser != null)
            {
                throw KariajiException.NewPublic("Ezt az email címet már regisztrálták");
            }

            var user = new User
            {
                Email = email.Trim(),
                Password = this.protectionService.HashPassword(password)

            };
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            return user;

        }


        public async Task<string> Register(string email)
        {
            var em = email.Trim().ToLower();
            var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == em);
            if (user != null)
            {
                throw KariajiException.NewPublic("Ezt az email címet már regisztrálták");
            }


            var token = this.protectionService.ProtecData(new RegisterTokenModel
            {
                Email = em,
                RequestDate = DateTime.Now
            });

            var link = Path.Combine(this.configSvc.Configuration.SiteBaseAddress, "confirm-registration?token=" + WebUtility.UrlEncode(token));

            await this.mailingSvc.SendEmailAsync("Kariaji regisztráció", $"Link: {link}", email);

            return link;
        }

        public async Task<string> GenerateToken(string email)
        {
            var em = email.Trim().ToLower();
            var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == em);
            if (user == null)
            {
                throw KariajiException.NewPublic("Hibás adatok");
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = "Kariaji",
                Audience = "Users",
                Expires = DateTime.UtcNow.AddDays(10),
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    //new Claim(ClaimTypes.Email, em)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(this.configSvc.Configuration.JWTKey)), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha512Digest)
            });

            var tokenStr = jwtHandler.WriteToken(token);
            //todo test: 
            //var isValid = this.Validatetoken(tokenStr, out int _id);
            return tokenStr;
        }

        public bool Validatetoken(string tokenString, out int id)
        {
            id = -1;
            var jwtHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;
            var param = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(1),
                ValidIssuer = "Kariaji",
                ValidAudience = "Users",
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(this.configSvc.Configuration.JWTKey)),
            };
            var claims = jwtHandler.ValidateToken(tokenString, param, out validatedToken);
            if (claims == null || validatedToken == null)
                return false;
            var nameIdClaim = claims.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (nameIdClaim == null)
                return false;

            id = int.Parse(nameIdClaim.Value);
            return true;




        }

     
    }
}
