using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public enum EmailTypes
    {
        GoogleMail = 1
    }

    public class Configuration
    {
        public int Id { get; set; }
        public string Version { get; set; } 
        public string AdminEmailAddress { get; set; }
        public string AdminEmailUserName { get; set; }
        public string AdminEmailFullName { get; set; }
        public EmailTypes AdminEmailType { get; set; }
        public string AdminEmailSMTPAddress { get; set; }
        public int AdminEmailSMTPPort { get; set; }
        public string AdminEmailUserPassword { get;set; }
        public string AdminUserName { get;set; }
        public string AdminUserPassword { get; set; }
        public string SiteBaseAddress { get; set; }

        public string JWTKey { get; set; }
        public string PasswordSalt { get; set; }


      
    }
}
