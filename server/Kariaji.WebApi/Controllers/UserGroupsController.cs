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
            this.InvalidateFriendUsersAndGroups();
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
            //var group = await this.ugSvc.GetGroupByIdAsync(groupId);
            //if (group == null)
            //    return BadRequest(CommonResult.NewError("A csoport nem található"));

            //if (!group.Memberships.Any(m => !m.IsDeleted && m.UserId == CurrentUser.Id))
            //    return BadRequest(CommonResult.NewError("Nem tagja ennek a csoportnak"));

            //return Ok(group.ToInfo());
            if (!this.FriendGroupIds.Contains(groupId))
                return BadRequest(CommonResult.NewError("Nem vagy tagja ennek a csoportnak"));
            var group = await this.ugSvc.GetGroupByIdAsync(groupId);
            return group.ToInfo();
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
        public async Task<ActionResult> RejectInvitation(int invitationId)
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
        public async Task<ActionResult> GetAvatar(int userId)
        {
            //if (userId != CurrentUser.Id && (!(await this.ugSvc.AreUsersFriends(userId, CurrentUser.Id))))
            //    throw KariajiException.NotAuthorized;
            //var avatar = await this.ugSvc.GetAvatarOfUserAsync(userId);
            //if (avatar != null)
            //    return File(avatar.Data, avatar.ContentType);
            //else
            //    return NoContent();
            if ((userId != CurrentUser.Id) && !this.FriendUserIds.Contains(userId))
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
            return Ok((await this.ugSvc.GetUserById(userId)).ToCompactInfo());
        }

        [HttpGet]
        [Route("friends")]
        public async Task<ActionResult<FriendsData>> GetDataOfFriends()
        {
            return new FriendsData
            {
                FriendUsers = (await this.ugSvc.GetUsersByIds(this.FriendUserIds.ToList())).Select(u => u.ToCompactInfo()).ToList(),
                FriendGroups = (await this.ugSvc.GetContainerGroupsAsync(CurrentUser.Id)).Select(g => g.ToInfo()).ToList(),
                //FriendAvatars = (await this.ugSvc.GetAvatarsOfUsersAsync(this.FriendUserIds.ToList())).ToList()
            };
        }

        [HttpDelete]
        [Route("memberships")]
        public async Task DeleteMembership(int userId, int groupId)
        {
            if (await ugSvc.IsManagerOfUser(CurrentUser.Id, userId))
            {
                if (!await ugSvc.IsMemberOfGroup(groupId, CurrentUser.Id))
                    throw KariajiException.NotAuthorized;
            }
            else if (!(await ugSvc.CanAdministerGroup(groupId, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;
            if (userId == this.CurrentUser.Id)
                this.InvalidateFriendUsersAndGroups();
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

        [HttpPost]
        [Route("managed-users")]
        public async Task<ActionResult<CompactUserInfo>> CreateManagedUser(string displayName)
        {
            if (!await this.ugSvc.IsManagedUserNameFree(CurrentUser.Id, displayName))
                throw KariajiException.NewPublic("Már létezik ilyen nevű felügyelt fiókod");
            var user = await this.ugSvc.CreateManagedUserAsync(CurrentUser.Id, displayName);
            return user.ToCompactInfo();
        }

        [HttpDelete]
        [Route("managed-users/{managedUserId}")]
        public async Task<ActionResult> DeleteManagedUser(int managedUserId)
        {
            if (!await this.ugSvc.IsManagerOfUser(CurrentUser.Id, managedUserId))
                throw KariajiException.NewPublic("Nem törölheted ezt a fiókot!");
            await this.ugSvc.DeleteManagedUser(managedUserId);
            return Ok();
        }

        [HttpPost]
        [Route("managed-users/{managedUserId}/managers")]
        public async Task<ActionResult> AddManagerToUser(int managerUserId, int managedUserId)
        {
            if (!await this.ugSvc.IsManagerOfUser(CurrentUser.Id, managedUserId))
                throw KariajiException.NotAuthorized;

            if (await this.ugSvc.IsManagerOfUser(managerUserId, managedUserId))
                throw KariajiException.NewPublic("Már hozzáadtad!");

            await this.ugSvc.AddManagerToUser(managerUserId, managedUserId);
            return Ok();

        }


        [HttpDelete]
        [Route("managed-users/{managedUserId}/managers/{managerUserId}")]
        public async Task<ActionResult> RemoveManagerOfUser(int managerUserId, int managedUserId)
        {
            if (!await this.ugSvc.CanRemoveManagerOfUser(CurrentUser.Id, managerUserId, managedUserId))
                throw KariajiException.NotAuthorized;
            await this.ugSvc.RemoveManagerOfUser(managerUserId, managedUserId);
            return Ok();
        }

        [HttpPut]
        [Route("managed-users/{managedUserId}/groups/{groupId}")]
        public async Task<ActionResult> AddManagedUserToGroup(int managedUserId, int groupId)
        {
            if (!await ugSvc.IsManagerOfUser(CurrentUser.Id, managedUserId))
                throw KariajiException.NotAuthorized;
            if (!await ugSvc.IsMemberOfGroup(groupId, CurrentUser.Id))
                throw KariajiException.NotAuthorized;
            if (await ugSvc.IsMemberOfGroup(groupId, managedUserId))
                throw KariajiException.BadParamters;

            var m = await ugSvc.AddManagedUserToGroup(managedUserId, groupId);
            return Ok();
        }






    }
}
