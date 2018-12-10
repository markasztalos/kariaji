using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Mvc;

namespace Kariaji.WebApi.Controllers
{
    public class KariajiBaseController : ControllerBase
    {
        protected AuthenticationService authSvc;
        public KariajiBaseController(AuthenticationService authSvc)
        {
            this.authSvc = authSvc;
        }

        private User _CurrentUser;
        protected User CurrentUser
        {
            get
            {
                if (_CurrentUser == null && this.User != null)
                {
                    var nameIdClaimValue = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (nameIdClaimValue != null && int.TryParse(nameIdClaimValue, out int id))
                    {
                        this._CurrentUser = this.authSvc.GetUserById(id);
                    }
                }

                return _CurrentUser;
            }
        }
    }
}
