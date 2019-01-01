using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kariaji.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class GroupsController : KariajiBaseController
    {
        public GroupsController(UserGroupManagerService ugSvc) : base(ugSvc)
        {
        }
        
        [HttpPost]
        [Route("groups")]
        public Task<ActionResult<List<Group>>> CreateGroup(string name, string description)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("groups")]
        public Task<ActionResult<List<Group>>> GetMyGroups()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("groups/{groupId}")]
        public Task<ActionResult> RemoveGroup(int groupId)
        {
            throw new NotImplementedException();
        }
       
        [HttpGet]
        [Route("invitations")]
        public Task<ActionResult<List<UserGroupInvitation>>> GetMyInvitations()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("invitations")]
        public Task<ActionResult<bool>> InviteUserByAddress(int groupId, string emailAddress)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("invitations/accept/{invitationId}")]
        public Task<ActionResult> AcceptInvitation(int invitationId)
        {
            throw new NotImplementedException();
        }
        [HttpPut]
        [Route("invitations/reject/{invitationId}")]
        public Task<ActionResult> RejectInvitation(int invitationId)
        {
            throw new NotImplementedException();
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

        



    }
}
