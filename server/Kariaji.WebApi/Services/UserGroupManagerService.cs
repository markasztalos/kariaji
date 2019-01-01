using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Middlewares;

namespace Kariaji.WebApi.Services
{
    public class UserGroupManagerService
    {
        private readonly KariajiContext ctx;
        private readonly MailingService mailingSvc;
        private readonly ProtectionService protectionService;
        private readonly ConfigurationProviderService configSvc;
        
        public UserGroupManagerService(KariajiContext ctx, ProtectionService protectionService,
            MailingService mailingSvc, ConfigurationProviderService configSvc)
        {
            this.ctx = ctx;
            this.protectionService = protectionService;
            this.mailingSvc = mailingSvc;
            this.configSvc = configSvc;
        }


        public async Task<User> UpdateUserAccount(int userId, string displayName)
        {
            var user = this.ctx.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw KariajiException.NewPublic("A felhasználó nem létezik");
            
            user.DisplayName = displayName;
            await this.ctx.SaveChangesAsync();
            return user;
        }

        public async Task UpdatePassword(int userId, string password)
        {
            var user = this.ctx.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw KariajiException.NewPublic("A felhasználó nem létezik");
            user.Password = this.protectionService.HashPassword(password);
            await this.ctx.SaveChangesAsync();
        }

        public User GetUserById(int id)
        {
            return this.ctx.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
