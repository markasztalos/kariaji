using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Microsoft.EntityFrameworkCore;

namespace Kariaji.WebApi.Services
{
    public class AuthenticationService
    {
        private KariajiContext ctx;

        public AuthenticationService(KariajiContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<bool> CheckPassword()
        {
            await this.ctx.Users.ToListAsync();
            return true;
        }
    }
}
