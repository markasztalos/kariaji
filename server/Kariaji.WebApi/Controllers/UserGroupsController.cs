using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kariaji.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UserGroupsController : KariajiBaseController
    {
        public UserGroupsController(UserGroupManagerService ugSvc) : base(ugSvc)
        {
        }

        [HttpPost]
        [Route("groups")]
        public async Task<ActionResult<List<Group>>> CreateGroup([FromBody] CreateNewGroupModel model)
        {
            var group = await this.ugSvc.CreateNewGroupAsync(model, CurrentUser.Id);
            return Ok(group.ToCompactInfo());
        }

        [HttpGet]
        [Route("memberships")]
        public async Task<ActionResult<List<OwnMembershipInfo>>> GetOwnMemberships()
        {
            var memberships = await this.ugSvc.GetMemberships(this.CurrentUser.Id);

            return Ok(memberships.Select(membership => membership.ToOwnMembershipInfo()));
        }

        [HttpDelete]
        [Route("groups/{groupId}")]
        public Task<ActionResult> RemoveGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("groups/{groupId}")]
        public async Task<ActionResult<GroupInfo>> GetGroup(int groupId)
        {
            var group = await this.ugSvc.GetGroupByIdAsync(groupId);
            if (group == null)
                return BadRequest(CommonResult.NewError("A csoport nem található"));

            if (!group.Memberships.Any(m => !m.IsDeleted && m.UserId == CurrentUser.Id))
                return BadRequest(CommonResult.NewError("Nem tagja ennek a csoportnak"));

            return Ok(group.ToInfo());
        }

        [HttpGet]
        [Route("groups/names")]
        public async Task<ActionResult<List<string>>> GetExistingGroupNames()
        {
            return await this.ugSvc.GetExistingGroupNames();
        }

        [HttpGet]
        [Route("users/emails")]
        public async Task<ActionResult<List<string>>> GetExistingEmailAddresses([FromQuery] int[] exceptUserIds = null)
        {
            return await this.ugSvc.GetExistingEmailAddressesAsync(exceptUserIds);
        }
        

        [HttpGet]
        [Route("groups/{groupId}/invitations")]
        public async Task<ActionResult<List<UserGroupInvitationInfo>>> GetInvitationsOfGroup(int groupId)
        {
            var invitations = await this.ugSvc.GetInvitationsOfGroupAsync(groupId, CurrentUser.Id);
            return invitations.Select(i => i.ToInfo()).ToList();
        }

        [HttpGet]
        [Route("invitations")]
        public async Task<ActionResult<List<UserGroupInvitationInfo>>> GetMyInvitations()
        {
            var invitations = await this.ugSvc.GetMyInvitations(CurrentUser.Id);
            return invitations.Select(i => i.ToInfo()).ToList();
        }

        [HttpPost]
        [Route("invitations")]
        public async Task<ActionResult> InviteUserByAddress([FromBody] UserGroupInvitationModel model)
        {
            await this.ugSvc.InviteAsync(model.GroupId, model.Email, this.CurrentUser.Id);
            return Ok();
        }

        [HttpGet]
        [Route("invitations/{invitationId}/accept")]
        public async Task<ActionResult> AcceptInvitation(int invitationId)
        {
            await this.ugSvc.AcceptInvitationAsync(invitationId, CurrentUser.Id);
            return Ok();
        }
        [HttpGet]
        [Route("invitations/{invitationId}/reject")]
        public  async Task<ActionResult> RejectInvitation(int invitationId)
        {
            await this.ugSvc.RejectInvitationAsync(invitationId, CurrentUser.Id);
            return Ok();
        }

        [HttpDelete]
        [Route("invitations/{invitationId}")]
        public async Task<ActionResult> DeleteInvitation(int invitationId)
        {
            await this.ugSvc.DeleteInvitationAsync(invitationId, CurrentUser.Id);
            return Ok();
        }

        [HttpDelete]
        [Route("memberships")]
        public Task<ActionResult> RemoveUserFromGroup(int groupId, int userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("memberships/set-administration-rights")]
        public Task<ActionResult> AddGroupAdministrationRights(int groupId, int userId)
        {
            throw new NotImplementedException();
        }
        [HttpPut]
        [Route("memberships/remove-administration-rights")]
        public Task<ActionResult> RemoveGroupAdministrationRights(int groupId, int userId)
        {
            throw new NotImplementedException();
        }


        [HttpGet]
        [Route("user/{userId}/avatar")]
        public  async Task<ActionResult> GetAvatar(int userId)
        {
            if (userId != CurrentUser.Id && (!(await this.ugSvc.AreUsersFriends(userId, CurrentUser.Id))))
                throw KariajiException.NotAuthorized;
            var avatar = await this.ugSvc.GetAvatarOfUserAsync(userId);
            if (avatar != null)
                return File(avatar.Data, avatar.ContentType);
            else
                return NoContent();
        }

        [HttpGet]
        [Route("user/{userId}")]
        public async Task<ActionResult<CompactUserInfo>> GetUser(int userId)
        {
            if (userId != CurrentUser.Id && (!(await this.ugSvc.AreUsersFriends(userId, CurrentUser.Id))))
                throw KariajiException.NotAuthorized;
            return Ok(this.ugSvc.GetUserById(userId).ToCompactInfo());
        }

        [HttpDelete]
        [Route("memberships")]
        public async Task DeleteMembership(int userId, int groupId)
        {
            if (!(await ugSvc.CanAdministerGroup(groupId, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;

            await this.ugSvc.DeleteMembership(groupId, userId);
        }

        [HttpPut]
        [Route("memberships")]
        public async Task UpdateMembership([FromBody]UpdateMembershipModel model)
        {
            if (!(await ugSvc.CanAdministerGroup(model.GroupId, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;
            await this.ugSvc.UpdateMembership(model.GroupId, model.UserId, model.IsAdministrator);
        }

        [HttpGet]
        [Route("container-groups")]
        public async Task<List<GroupInfo>> GetContainerGroups()
        {
            var groups = await this.ugSvc.GetContainerGroupsAsync(CurrentUser.Id);
            return groups.Select(g => g.ToInfo()).ToList();
        }
    }
}
