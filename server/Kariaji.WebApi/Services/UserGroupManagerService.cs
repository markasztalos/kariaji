using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;

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


    }
}
