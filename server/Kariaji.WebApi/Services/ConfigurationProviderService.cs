using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Kariaji.WebApi.Services
{
    public class ConfigurationProviderService
    {
        private readonly ProtectionService protectionService;
        private readonly KariajiContext dbContext;
        public ConfigurationProviderService(ProtectionService protectionService, KariajiContext dbContext)
        {
            this.protectionService = protectionService;
            this.dbContext = dbContext;

        }

        private Configuration _DefaultConfiguration;

        public Configuration DefaultConfiguration => _DefaultConfiguration ?? (_DefaultConfiguration = new Configuration
        {
            AdminEmailAddress = "kariaji.web@gmail.com",
            Version = "1",
            AdminUserName = "admin",
            AdminUserPassword = this.protectionService.HashPassword("adminadmin"),
            AdminEmailSMTPAddress = "smtp.gmail.com",
            AdminEmailSMTPPort = 587,
            AdminEmailFullName = "Kariaji",
            AdminEmailUserName = "kariaji.web@gmail.com",
            AdminEmailType = EmailTypes.GoogleMail,
            AdminEmailUserPassword = this.protectionService.ProtectPassword("1024Archers"),
            SiteBaseAddress = "http://localhost:4208/",
            JWTKey = ProtectionService.JwtKey,
            PasswordSalt = ProtectionService.Salt

        });

        private Configuration _Configuration;

        public Configuration Configuration => this._Configuration ?? (this._Configuration = this._Configuration = dbContext.Configurations.First());

    }
}
