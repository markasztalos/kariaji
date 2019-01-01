using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kariaji.WebApi.Controllers
{
    [Authorize]
    [Route("api/my-account")]
    public class AccountController : KariajiBaseController
    {
        public AccountController(UserGroupManagerService ugSvc) : base(ugSvc)
        {
        }

        [HttpPut]
        public async Task<ActionResult<UserInfo>> UpdateAccount([FromBody]UpdateMyAccountModel model)
        {
            if (!this.ModelState.IsValid)
                return BadRequest(CommonResult.NewError());

            var user = await this.ugSvc.UpdateUserAccount(this.CurrentUser.Id, model.DisplayName);
            return Ok(user.ToInfo());
        }

        [HttpPut]
        [Route("password")]
        public async Task<ActionResult<CommonResult>> UpdatePassword([FromBody] UpdatePasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(CommonResult.NewError("Kötelező jelszót megadni"));
            }

            await this.ugSvc.UpdatePassword(this.CurrentUser.Id, model.Password);

            return Ok(CommonResult.NewSuccess());
        }

        [HttpDelete]
        public Task<ActionResult> DeleteAccount()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<ActionResult<UserInfo>> GetMyAccount()
        {
            return Ok(this.CurrentUser.ToInfo());
        }
    }
}
