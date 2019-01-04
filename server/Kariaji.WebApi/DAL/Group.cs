using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class Group
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }

        public DateTime CreationDate {get; set; }
        public bool IsDeleted { get; set; }

        public User CreatorUser {get; set; }
        public int CreatorUserId { get; set; }

        public ICollection<Membership> Memberships { get; set; }

        public ICollection<IdeaTargetGroup> IdeaTargets { get; set; }

    }

    public class Membership
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserGroupInvitation
    {
        public int Id { get; set; }
        public int GroupId {get; set; }
        public Group Group { get; set; }
        public User InvitedUser { get; set; }
        public int? InvitedUserId { get; set; }
        public User SenderUser { get;set; }
        public int SenderUserId { get;set; }
        public string InvitedEmail { get; set; }
        public DateTime SendingDate { get; set; }
    }
}
