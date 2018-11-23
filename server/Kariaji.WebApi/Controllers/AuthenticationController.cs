using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kariaji.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private KariajiContext ctx;
        public AuthenticationController(KariajiContext ctx)
        {
            this.ctx = ctx;
        }

        [HttpGet("test")]
        public async Task<ActionResult<int>> Test()
        {
            var users = this.ctx.Users.ToList();
            return users.Count;
        }
    }
}