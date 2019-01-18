using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Helpers;
using Microsoft.AspNetCore.Http;

namespace Kariaji.WebApi.Models
{
    public static class UserGroupConverterter
    {
        public static UserGroupInvitationInfo ToInfo(this UserGroupInvitation invitation) => new UserGroupInvitationInfo
        {
            Id = invitation.Id,
            GroupId = invitation.GroupId,
            InvitedEmail = invitation.InvitedEmail,
            GroupDisplayName = invitation.Group.DisplayName,
            InvitedUser = invitation.InvitedUser.ToCompactInfo(),
            SenderUser = invitation.SenderUser.ToCompactInfo(),
            SendingDate = invitation.SendingDate.ToHungarianDateTime()
        };

        public static CompactGroupInfo ToCompactInfo(this Group group) => new CompactGroupInfo
        {
            Id = group.Id,
            DisplayName = group.DisplayName,
            CreationDate = group.CreationDate.ToHungarianDate(),
            CreatorUserDisplayName = group.CreatorUser.DisplayName,
            Description = group.Description
        };

        public static GroupInfo ToInfo(this Group group) => new GroupInfo
        {
            Id = group.Id,
            DisplayName = group.DisplayName,
            CreationDate = group.CreationDate.ToHungarianDate(),
            CreatorUserDisplayName = group.CreatorUser.DisplayName,
            Description = group.Description,
            Members = group.Memberships.Where(m => !m.IsDeleted).Select(m => new GroupMemberInfo
            {
                IsAdministrator = m.IsAdministrator,
                User = m.User.ToCompactInfo()
            }).ToList()
        };

        public static CompactUserInfo ToCompactInfo(this User user) => new CompactUserInfo
        {
            DisplayName = string.IsNullOrEmpty(user.DisplayName) ? user.Email : user.DisplayName,
            Email = user.Email,
            Id = user.Id
        };

        public static OwnMembershipInfo ToOwnMembershipInfo(this Membership membership)
        {
            return new OwnMembershipInfo
            {
                GroupId = membership.Group.Id,
                GroupDisplayName = membership.Group.DisplayName,
                IsAdministrator = membership.IsAdministrator,
                GroupDescription = membership.Group.Description
            };
        }
    }

    public class CompactUserInfo
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }

    public class CompactGroupInfo
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string CreationDate { get; set; }
        public string CreatorUserDisplayName { get; set; }
        public string Description { get; set; }
    }

    public class GroupInfo : CompactGroupInfo
    {
        public List<GroupMemberInfo> Members { get; set; }
    }

    public class GroupMemberInfo
    {
        public CompactUserInfo User { get; set; }
        public bool IsAdministrator { get; set; }
    }

    public class OwnMembershipInfo
    {
        public int GroupId { get; set; }
        public string GroupDisplayName { get; set; }
        public bool IsAdministrator { get; set; }
        public string GroupDescription { get; set; }
    }

    public class CreateNewGroupModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserGroupInvitationInfo
    {
        public int Id { get; set; }
        public int GroupId {get; set; }
        public string GroupDisplayName { get; set; }
        public CompactUserInfo InvitedUser { get; set; }
        public CompactUserInfo SenderUser { get; set; }
        public string InvitedEmail { get; set; }
        public string SendingDate { get; set; }
    }

    public class UserGroupInvitationModel
    {
        public int GroupId { get; set; }
        public string Email { get; set; }
    }

    public class UpdateMembershipModel
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public bool IsAdministrator { get; set; }
    }

    public class FriendsData
    {
        public List<CompactUserInfo> FriendUsers { get; set; }
        public List<GroupInfo> FriendGroups { get; set; }
        //public List<Avatar> FriendAvatars {get; set; }
    }

 

}
