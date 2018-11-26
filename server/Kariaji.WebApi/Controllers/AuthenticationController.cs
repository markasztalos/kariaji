using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kariaji.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private KariajiContext ctx;
        private AuthenticationService authSvc;
        public AuthenticationController(KariajiContext ctx, AuthenticationService authSvc)
        {
            this.ctx = ctx;
            this.authSvc = authSvc;
        }

        [HttpGet("login")]
        public async Task<ActionResult<bool>> login()
        {
            //var users = await this.ctx.Users.ToListAsync();
            //return users.Count;
            return await this.authSvc.CheckPassword();

        }
    }
}