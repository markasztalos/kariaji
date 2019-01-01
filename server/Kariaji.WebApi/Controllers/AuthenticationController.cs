using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kariaji.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : KariajiBaseController
    {
        private AuthenticationService authSvc;
        public AuthenticationController(UserGroupManagerService ugSvc, AuthenticationService authSvc) : base(ugSvc)
        {
            this.authSvc = authSvc;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResultModel>> Login([FromBody]LoginModel model)
        {
            //var users = await this.ctx.Users.ToListAsync();
            //return users.Count;

            await this.authSvc.CheckPassword(model.Email, model.Password);
            return Ok(new LoginResultModel
            {
                Token = await this.authSvc.GenerateToken(model.Email)
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResultModel>> Register([FromBody] RegisterModel model)
        {
            var link = await this.authSvc.Register(model.Email);
            return Ok(new RegisterResultModel
            {
                Link = link
            });
        }

        [HttpPost("confirm-registration")]
        public async Task<ActionResult> ConfirmRegistration([FromBody] ConfirmRegistrationModel model)
        {
            await this.authSvc.ConfirmRegistration(model.Token, model.Password);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("test")]
        public async Task<ActionResult> Test()
        {

            return Ok(this.CurrentUser.Email);
        }
    }
}