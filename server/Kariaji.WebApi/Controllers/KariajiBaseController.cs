using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Mvc;

namespace Kariaji.WebApi.Controllers
{
    public class KariajiBaseController : ControllerBase
    {
        protected UserGroupManagerService ugSvc;
        public KariajiBaseController(UserGroupManagerService ugSvc)
        {
            this.ugSvc = ugSvc;
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
                        this._CurrentUser = this.ugSvc.GetUserById(id);
                    }
                }

                return _CurrentUser;
            }
        }

 
        protected bool IsFriendUser(int userId)=> this.FriendUsers.Contains(userId);
        protected bool IsFriendGroup(int groupId) => this.FriendGroups.Contains(groupId);

        private HashSet<int> _FriendGroups;
        private HashSet<int> _FriendUsers;

        protected void InvalidateFriendUsersAndGroups()
        {
            this._FriendGroups = null;
            this._FriendUsers = null;
        }

        private HashSet<int> FriendGroups
        {
            get
            {
                if (this._FriendGroups == null)
                {
                    this._FriendGroups = this.ugSvc.GetContainerGroupsAsync(CurrentUser.Id).Result.Select(g => g.Id).ToHashSet();
                }

                return _FriendGroups;
            }
        }

        public HashSet<int> FriendUsers
        {
            get
            {
                if (this._FriendUsers == null)
                {
                    this._FriendUsers = this.ugSvc.GetContainerGroupsAsync(CurrentUser.Id).Result
                        .SelectMany(g => g.Memberships.Where(m => !m.IsDeleted).Select(m => m.UserId)).ToHashSet();
                }

                return _FriendUsers;
            }
        }
    }
}
