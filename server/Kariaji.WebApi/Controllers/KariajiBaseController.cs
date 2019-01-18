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
                        this._CurrentUser = this.ugSvc.GetUserById(id).Result;
                    }
                }

                return _CurrentUser;
            }
        }


        protected bool IsFriendUser(int userId) => this.FriendUserIds.Contains(userId);
        protected bool IsFriendGroup(int groupId) => this.FriendGroupIds.Contains(groupId);

        private HashSet<int> _FriendUsersIds;

        public HashSet<int> FriendUserIds
        {
            get
            {
                if (_FriendUsersIds == null)
                    _FriendUsersIds = this.ugSvc.GetFriendUserIds(CurrentUser.Id).Result.ToHashSet();
                return _FriendUsersIds;
            }
        }

        protected void InvalidateFriendUsersAndGroups()
        {
            this._FriendGroupsIds = null;
            this._FriendUsersIds = null;
        }

        private HashSet<int> _FriendGroupsIds = null;

        public HashSet<int> FriendGroupIds
        {
            get
            {
                if (_FriendGroupsIds == null)
                    _FriendGroupsIds = this.ugSvc.GetContainerGroupIdsAsync(this.CurrentUser.Id).Result.ToHashSet();
                return _FriendGroupsIds;
            }
        }

        //public Dictionary<int, GroupInfo> FriendGroups
        //{
        //    get
        //    {
        //        if (this._FriendGroups == null)
        //        {
        //            this._FriendGroups = this.ugSvc.GetContainerGroupsAsync(CurrentUser.Id).Result
        //                .ToDictionary(g => g.Id, g => g.ToInfo());
        //        }

        //        return _FriendGroups;
        //    }
        //}

        //public Dictionary<int, (CompactUserInfo user, Avatar avatar)> FriendUsers
        //{
        //    get
        //    {
        //        if (this._FriendUsers == null)
        //        {
        //            var friends = this.ugSvc.GetFriendUsers(CurrentUser.Id).Result;
        //            this._FriendUsers = friends.ToDictionary(u => u.Id, u => (u.ToCompactInfo(), u.Avatar));
        //        }

        //        return _FriendUsers;
        //    }
        //}


    }
}
