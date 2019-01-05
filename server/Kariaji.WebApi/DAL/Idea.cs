using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.DAL
{
    public class Idea
    {
        public int Id { get; set; }

        public int CreatorUserId { get; set; }
        public User CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }

        public string TextDelta { get; set; }

        public ICollection<IdeaTargetGroup> TargetGroups { get; set; }
        public ICollection<IdeaUser> Users { get; set; }

        public int? ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public ICollection<IdeaComment> Comments { get; set; }

        public bool GotIt { get; set; }
    }

    public class IdeaTargetGroup
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
    }

    public class IdeaUser
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
        public bool IsSecret { get; set; }
    }
    public class IdeaComment
    {
        public int Id { get; set; }
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
        public DateTime CreationTime { get; set; }
        public string TextDelta { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
