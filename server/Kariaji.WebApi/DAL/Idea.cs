using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class Idea
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(CreatorUser))]
        public int CreatorUserId { get; set; }
        public User CreatorUser { get; set; }

        public DateTime CreationTime { get; set; }

        public string TextDelta { get; set; }

        [InverseProperty(nameof(IdeaTargetGroup.Idea))]
        public ICollection<IdeaTargetGroup> TargetGroups { get; set; }

        [InverseProperty(nameof(IdeaUser.Idea))]
        public ICollection<IdeaUser> Users { get; set; }

        //[ForeignKey(nameof(Reservation))]
        //public int? ReservationId { get; set; }
        [InverseProperty(nameof(Kariaji.WebApi.DAL.Reservation.Idea))]
        public Reservation Reservation { get; set; }

        [InverseProperty(nameof(IdeaComment.Idea))]
        public ICollection<IdeaComment> Comments { get; set; }

        public bool GotIt { get; set; }
    }

    public class IdeaTargetGroup
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        public Group Group { get; set; }
        [ForeignKey(nameof(Idea))]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
    }

    public class IdeaUser
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Idea))]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
        public bool IsSecret { get; set; }
    }
    public class IdeaComment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Idea))]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
        public DateTime CreationTime { get; set; }
        public string TextDelta { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
