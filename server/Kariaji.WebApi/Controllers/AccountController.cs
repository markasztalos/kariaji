using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<CompactUserInfo>> UpdateAccount([FromBody]UpdateMyAccountModel model)
        {
            if (!this.ModelState.IsValid)
                return BadRequest(CommonResult.NewError());

            var user = await this.ugSvc.UpdateUserAccount(this.CurrentUser.Id, model.DisplayName);
            return Ok(user.ToCompactInfo());
        }

        [HttpPut]
        [Route("password")]
        public async Task<ActionResult<CommonResult>> UpdatePassword([FromBody] UpdatePasswordModel model)
        {
            await this.ugSvc.UpdatePasswordAsync(this.CurrentUser.Id, model.OldPassword, model.NewPassword);

            return Ok(CommonResult.NewSuccess());
        }

        [HttpDelete]
        public Task<ActionResult> DeleteAccount()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<ActionResult<CompactUserInfo>> GetMyAccount()
        {
            return Ok(this.CurrentUser.ToCompactInfo());
        }

        [HttpPut]
        [Route("avatar")]
        public async Task<ActionResult> UpdateAvatar([FromForm] IFormFile image)
        {
            var avatar = new Avatar
            {
                UserId = CurrentUser.Id,
                ContentType = image.ContentType
            };
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                avatar.Data = memoryStream.ToArray();
                
            }
            await this.ugSvc.UpdateAvatarAsync(avatar);
            return Ok();
        }

        [HttpDelete]
        [Route("avatar")]
        public async Task<ActionResult> DeleteAvatar()
        {
            await this.ugSvc.DeleteAvatar(CurrentUser.Id);
            return Ok();
        }
    }
}
