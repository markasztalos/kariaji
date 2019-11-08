using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.Models
{
    public class ForgotPasswordTokenModel
    {
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Email { get; set; }
    }

    public class PasswordRecoveryModel
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class RegisterTokenModel
    {
        public string Email { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
    }

    public class RegisterResultModel
    {
        public string Link { get; set; }
    }

    public class ConfirmRegistrationModel
    {
        public string Token { get;set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResultModel
    {
        public string Token { get;set; }
    }
}
