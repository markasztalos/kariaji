using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;

namespace Kariaji.WebApi.Services
{
    public class EmailServiceConfiguration
    {
        public string AdminEmailAddress { get; set; }
        public string AdminEmailUserName { get; set; }
        public string AdminEmailFullName { get; set; }
        public EmailTypes AdminEmailType { get; set; }
        public string AdminEmailSMTPAddress { get; set; }
        public int AdminEmailSMTPPort { get; set; }
        public string AdminEmailUserPassword { get; set; }
    }

    public class MailingService
    {
        private readonly ProtectionService protectionService;
        private readonly KariajiContext dbContext;
        private readonly ConfigurationProviderService configProviderSvc;

        public MailingService(KariajiContext dbContext, ProtectionService protectionService, ConfigurationProviderService configProviderSvc)
        {
            this.dbContext = dbContext;
            this.protectionService = protectionService;
            this.configProviderSvc = configProviderSvc;
        }

        private EmailServiceConfiguration LoadConfiguration()
        {
            var dbConfig = this.configProviderSvc.Configuration;
            return new EmailServiceConfiguration
            {
                AdminEmailUserName = dbConfig.AdminEmailUserName,
                AdminEmailSMTPPort = dbConfig.AdminEmailSMTPPort,
                AdminEmailType = dbConfig.AdminEmailType,
                AdminEmailSMTPAddress = dbConfig.AdminEmailSMTPAddress,
                AdminEmailAddress = dbConfig.AdminEmailAddress,
                AdminEmailUserPassword = this.protectionService.UnprotectPassword(dbConfig.AdminEmailUserPassword),
                AdminEmailFullName = dbConfig.AdminEmailFullName
            };
        }
        private EmailServiceConfiguration _Configuration;

        private EmailServiceConfiguration Configuration =>
            _Configuration ?? (_Configuration = LoadConfiguration());


        private async Task SendGoogleMailAsync(string subject, string message, params string[] receiverAddresses)
        {
            try
            {
                var client = new SmtpClient
                {
                    Host = this.Configuration.AdminEmailSMTPAddress,
                    Port = this.Configuration.AdminEmailSMTPPort,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(this.Configuration.AdminEmailUserName,
                        this.Configuration.AdminEmailUserPassword)
                };
                var msg = new MailMessage
                {
                    From = new MailAddress(Configuration.AdminEmailAddress, Configuration.AdminEmailFullName),
                    Subject = subject,
                    Body = message
                };
                foreach (var receiver in receiverAddresses)
                    msg.To.Add(new MailAddress(receiver));
                //DEBUG: msg.To.Add(new MailAddress("mark.asztalos.dev@gmail.com"));
                client.EnableSsl = true;
                await client.SendMailAsync(msg);
            }
            catch
            {
                //todo log
                Console.WriteLine("Nem sikerült elküldeni");
            }
        }

        public async Task SendEmailAsync(string subject, string message, params string[] receiverAddresses)
        {

            switch (this.Configuration.AdminEmailType)
            {
                case EmailTypes.GoogleMail:
                    {
                        await SendGoogleMailAsync(subject, message, receiverAddresses);
                        return;
                    }
            }

            throw new Exception("Email szolgáltatás nincs megfelelően konfigurálva");
        }
    }
}
