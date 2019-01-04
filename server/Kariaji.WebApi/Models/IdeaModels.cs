using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Helpers;

namespace Kariaji.WebApi.Models
{
    public static class IdeasConverter
    {
        public static CompactIdeaInfo ToCompactInfo(this Idea idea, Predicate<int> groupFilter, Predicate<int> userFilter) => new CompactIdeaInfo
        {
            Id = idea.Id,
            TargetGroupIds = idea.TargetGroups.Select(g => g.GroupId).Where(u => groupFilter(u)).ToList(),
            TargetUserIds = idea.Users.Where(u => !u.IsSecret).Select(u => u.UserId).Where(u => userFilter(u)).ToList(),
            SecretUserIds = idea.Users.Where(u => u.IsSecret).Select(u => u.UserId).Where(u => userFilter(u)).ToList(),
            CreatorUserId = idea.CreatorUserId,
            CreationTime = idea.CreationTime.ToHungarianDateTime(),
            TextDelta = idea.TextDelta
        };

        public static IdeaInfo ToInfo(this Idea idea, Predicate<int> groupFilter, Predicate<int> userFilter, bool noDetails = false) => new IdeaInfo
        {
            Id = idea.Id,
            TargetGroupIds = idea.TargetGroups.Select(g => g.GroupId).Where(u => groupFilter(u)).ToList(),
            TargetUserIds = idea.Users.Where(u => !u.IsSecret).Select(u => u.UserId).Where(u => userFilter(u)).ToList(),
            SecretUserIds = idea.Users.Where(u => u.IsSecret).Select(u => u.UserId).Where(u => userFilter(u)).ToList(),
            CreatorUserId = idea.CreatorUserId,
            CreationTime = idea.CreationTime.ToHungarianDateTime(),
            TextDelta = idea.TextDelta,
            IsReserved = !noDetails && (idea.Reservation != null),
            Reservation = noDetails ? null :
                (idea.Reservation != null && userFilter(idea.Reservation.ReserverUserId) ? idea.Reservation.ToInfo() : null),
            Comments = !noDetails ? idea.Comments?.Select(c => c.ToInfo()).Where(c => userFilter(c.UserId)).ToList() : new List<IdeaCommentInfo>()
        };

        public static ReservationInfo ToInfo(this Reservation reservation) => new ReservationInfo
        {
            CanJoin = reservation.CanJoin,
            Id = reservation.Id,
            ReserverUserId = reservation.ReserverUserId,
            ReservationTime = reservation.ReservationTime.ToHungarianDateTime(),
            JoinedUserIds = reservation.Joins.Select(j => j.UserId).ToList()

        };

        public static IdeaCommentInfo ToInfo(this IdeaComment comment) => new IdeaCommentInfo
        {
            Id = comment.Id,
            UserId = comment.UserId,
            CreationTime = comment.CreationTime.ToHungarianDateTime(),
            TextDelta = comment.TextDelta
        };
    }

    public class CompactIdeaInfo
    {
        public int Id { get; set; }
        public int CreatorUserId { get; set; }
        public string CreationTime { get; set; }

        public string TextDelta { get; set; }

        public List<int> TargetGroupIds { get; set; }
        public List<int> TargetUserIds { get; set; }
        public List<int> SecretUserIds { get; set; }
    }

    public class IdeaInfo : CompactIdeaInfo
    {
        public ReservationInfo Reservation { get; set; }
        public bool IsReserved { get; set; }
        public List<IdeaCommentInfo> Comments { get; set; }
    }

    public class IdeaCommentInfo
    {
        public int Id { get; set; }
        public string CreationTime { get; set; }
        public string TextDelta { get; set; }
        public int UserId { get; set; }
    }

    public class ReservationInfo
    {
        public int Id { get; set; }
        public int ReserverUserId { get; set; }
        public string ReservationTime { get; set; }
        public bool CanJoin { get; set; }
        public List<int> JoinedUserIds { get; set; }
    }



    public class CreateIdeaModel
    {
        public string TextDelta { get; set; }
        public List<int> TargetGroupIds { get; set; }
        public List<int> TargetUserIds { get; set; }
        public List<int> SecretUserIds { get; set; }
    }

    public class CreateCommentModel
    {
        public string TextDelta { get; set; }
    }
}

