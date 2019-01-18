using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey(nameof(CreatorUser))]
        public int CreatorUserId { get; set; }

        [InverseProperty(nameof(Membership.Group))]
        public ICollection<Membership> Memberships { get; set; }

        [InverseProperty(nameof(IdeaTargetGroup.Group))]
        public ICollection<IdeaTargetGroup> IdeaTargets { get; set; }

    }

    public class Membership
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserGroupInvitation
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Group))]
        public int GroupId {get; set; }
        public Group Group { get; set; }
        
        public User InvitedUser { get; set; }
        [ForeignKey(nameof(InvitedUser))]
        public int? InvitedUserId { get; set; }

        public User SenderUser { get;set; }
        [ForeignKey(nameof(SenderUserId))]
        public int SenderUserId { get;set; }
        public string InvitedEmail { get; set; }
        public DateTime SendingDate { get; set; }
    }
}
