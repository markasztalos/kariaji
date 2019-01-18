using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Kariaji.WebApi.Services
{
    public class IdeasManagerService
    {
        private readonly KariajiContext ctx;
        private readonly MailingService mailingSvc;
        private readonly ProtectionService protectionService;
        private readonly ConfigurationProviderService configSvc;
        private readonly UserGroupManagerService ugSvc;

        public IdeasManagerService(KariajiContext ctx, ProtectionService protectionService,
            MailingService mailingSvc, ConfigurationProviderService configSvc, UserGroupManagerService ugSvc)
        {
            this.ctx = ctx;
            this.protectionService = protectionService;
            this.mailingSvc = mailingSvc;
            this.configSvc = configSvc;
            this.ugSvc = ugSvc;
        }

        public async Task<Idea> CreateNewIdeaAsyn(CreateIdeaModel model, int userId)
        {
            //var groups = await this.ugSvc.GetContainerGroupsAsync(userId);
            var idea = new Idea
            {
                TextDelta = model.TextDelta,
                CreationTime = DateTime.Now,
                CreatorUserId = userId,
            };
            this.ctx.Ideas.Add(idea);
            idea.TargetGroups = model.TargetGroupIds.Select(gid => new IdeaTargetGroup
            {
                //Group = groups.FirstOrDefault(g => g.Id == gid),
                GroupId = gid,
                Idea = idea
            }).ToList();
            foreach (var group in idea.TargetGroups)
                ctx.IdeaTargetGroups.Add(group);
            idea.Users = model.TargetUserIds.Select(uid => new IdeaUser
            {
                UserId = uid,
                Idea = idea,
                IsSecret = false
            }).Concat(model.SecretUserIds.Select(uid => new IdeaUser
            {
                UserId = uid,
                Idea = idea,
                IsSecret = true
            })).ToList();
            foreach (var user in idea.Users)
                ctx.IdeaUsers.Add(user);
            await this.ctx.SaveChangesAsync();
            return idea;
        }

        //public async Task<bool> CanAccessIdea(int userId, int ideaId)
        //{
        //    return await ctx.Ideas.AnyAsync(i =>
        //        i.Id == ideaId &&
        //        i.TargetGroups.Any(g =>
        //            g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
        //        i.Users.All(u => u.UserId != userId)
        //        );
        //}

        public async Task<bool> CanReserve(int userId, int ideaId)
        {
            return await ctx.Ideas.AnyAsync(i =>
                i.Id == ideaId &&
                i.TargetGroups.Any(g =>
                    g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
                i.Users.All(u => u.UserId != userId) &&
                (i.Reservation == null)
            );
        }
        public async Task<bool> CanUpdateGotIt(int userId, int ideaId)
        {
            return await ctx.Ideas.AnyAsync(i =>
                i.Id == ideaId &&
                i.TargetGroups.Any(g =>
                    g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
                i.Users.All(u => u.UserId != userId)
            );
        }

        public async Task<Reservation> Reserve(int userId, int ideaId)
        {
            var idea = await this.ctx.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId);
            if (idea == null)
                throw KariajiException.BadParamters;
            if (idea.Reservation != null)
                throw KariajiException.NewPublic("Ezt az ötletet már lefoglalták");

            var reservation = new Reservation
            {
                CanJoin = false,
                IdeaId = ideaId,
                ReservationTime = DateTime.Now,
                ReserverUserId = userId
            };
            ctx.Reservations.Add(reservation);
            await ctx.SaveChangesAsync();
            return reservation;
        }

        //public async Task<Reservation> GetReservation(int reservationId)
        //{
        //    return await ctx.Reservations.Include(r => r.Joins).FirstOrDefaultAsync(r => r.Id == reservationId);
        //}
        public async Task<int> GetReservationCreatorId(int reservationId)
        {
            return await ctx.Reservations.Where(r => r.Id == reservationId).Select(r => r.ReserverUserId).FirstOrDefaultAsync();
        }

        public async Task UpdateIfCanJoinToReservation(int reservationId, bool canJoin)
        {
            var reservaton = await ctx.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservaton == null)
                throw KariajiException.BadParamters;
            reservaton.CanJoin = canJoin;
            await ctx.SaveChangesAsync();
        }

        public async Task<bool> CanJoinToReservation(int reservationId, int userId)
        {
            return await ctx.Reservations.AnyAsync(r =>
                r.Id == reservationId &&
                r.ReservationJoins.All(j => j.UserId != userId) &&
                r.Idea.TargetGroups.Any(g =>
                    g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
                r.Idea.Users.All(u => u.UserId != userId));
        }

        public async Task<ReservationJoin> JoinToReservation(int reservationId, int userId)
        {
            var join = new ReservationJoin
            {
                ReservationId = reservationId,
                UserId = userId
            };
            ctx.ReservationJoins.Add(join);
            await ctx.SaveChangesAsync();
            return join;
        }

        public async Task<bool> CanRemoveJoin(int reservationId, int currentUserId, int removeUserId)
        {
            return await ctx.ReservationJoins.AnyAsync(j =>
                j.ReservationId == reservationId &&
                (j.UserId == removeUserId &&
                 (j.Reservation.ReserverUserId == currentUserId || currentUserId == removeUserId)));

        }

        public async Task RemoveJoin(int reservationId, int userId)
        {
            var join = await ctx.ReservationJoins.FirstOrDefaultAsync(j => j.ReservationId == reservationId && j.UserId == userId);
            ctx.ReservationJoins.Remove(join);
            await ctx.SaveChangesAsync();
        }

        public async Task<Idea> GetIdeaById(int ideaId)
        {
            return await ctx.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId);
        }



        public async Task<List<Idea>> GetVisibleIdeas(int userId,
            IReadOnlyList<int> filteredTargetGroupIds,
            IReadOnlyList<int> filteredTargetUserIds,
            bool onlyNotReserved,
            bool onlyReservedByMe,
            bool onlySentByMe,
            int skip, int take
            )
        {
            var query = GetQueryForVisibleIdeas(userId, filteredTargetGroupIds, filteredTargetUserIds, onlyNotReserved, onlyReservedByMe, onlySentByMe);
            return await query.Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetVisibleIdeasCount(int userId,
            IReadOnlyList<int> filteredTargetGroupIds,
            IReadOnlyList<int> filteredTargetUserIds,
            bool onlyNotReserved,
            bool onlyReservedByMe,
        bool onlySentByMe
        )
        {
            var query = GetQueryForVisibleIdeas(userId, filteredTargetGroupIds, filteredTargetUserIds, onlyNotReserved, onlyReservedByMe, onlySentByMe);
            return await query.CountAsync();
        }

        private IQueryable<Idea> GetQueryForVisibleIdeas(int userId, IReadOnlyList<int> filteredTargetGroupIds, IReadOnlyList<int> filteredTargetUserIds, bool onlyNotReserved, bool onlyReservedByMe, bool onlySentByMe)
        {
            var query = ctx.Ideas
                .Include(i => i.Reservation).ThenInclude(r => r.ReservationJoins)
                .Include(i => i.TargetGroups)
                .Include(i => i.Users)
                .Include(i => i.Comments)
                .Where(i =>
                    (i.CreatorUserId == userId ||
                    (i.TargetGroups.Any(g => g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
                     i.Users.All(u => u.UserId != userId))) &&
                    //(filteredTargetGroupIds == null || filteredTargetGroupIds.Any(g => i.TargetGroups.Any(g2 => g2.GroupId == g))) &&
                    //(filteredTargetUserIds == null || filteredTargetUserIds.Any(g => i.Users.Where(u => !u.IsSecret).Any(u => u.UserId == g))) &&
                    (!onlyNotReserved || (i.Reservation == null || i.Reservation.ReserverUserId == userId)) &&
                    (!onlyReservedByMe || (i.Reservation != null) && i.Reservation.ReserverUserId == userId) &&
                    (!onlySentByMe || i.CreatorUserId == userId)
                );

            if (filteredTargetGroupIds != null)
            {
                query = query.Where(i => i.TargetGroups.Any(g => filteredTargetGroupIds.Contains(g.GroupId)));
            }

            if (filteredTargetUserIds != null)
            {
                //query = query.Where(i => filteredTargetUserIds.Any(g => i.Users/*.Where(u => !u.IsSecret)*/.Any(u => u.UserId == g)));
                query = query.Where(i => i.Users.Where(u => !u.IsSecret).Any(u => filteredTargetUserIds.Contains(u.UserId)));
            }
            query = query.OrderByDescending(i => i.CreationTime);
            return query;
        }


        public async Task<List<Idea>> GetVisibleIdeasExceptMine(int userId)
        {
            return await ctx.Ideas
                .Include(i => i.Reservation).ThenInclude(r => r.ReservationJoins)
                .Include(i => i.TargetGroups).Include(i => i.Users)
                .Where(i =>
                    i.TargetGroups.Any(g => g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
                    i.Users.All(u => u.UserId != userId)
                    )
                .ToListAsync();
        }

        public async Task<List<Idea>> GetOwnIdeas(int userId)
        {
            return await ctx.Ideas
                .Where(i => i.CreatorUserId == userId && i.Users.Any(u => u.UserId == userId))
                .ToListAsync();
        }


        public async Task DeleteReservaion(int reservationId)
        {
            var joins = await ctx.ReservationJoins.Where(j => j.ReservationId == reservationId).ToListAsync();
            if (joins.Any())
                ctx.ReservationJoins.RemoveRange(joins);
            ctx.Reservations.Remove(new Reservation { Id = reservationId });
            await ctx.SaveChangesAsync();

        }

        public async Task<bool> CanDeleteIdea(int ideaId, int userId)
        {
            return await ctx.Ideas.AnyAsync(i => i.Id == ideaId && i.CreatorUserId == userId);
        }

        public async Task DeleteIdea(int ideaId)
        {
            var command = $"DELETE FROM IdeaComment WHERE IdeaId = {ideaId} GO " +
                          $"DELETE FROM IdeaUser WHERE IdeaId = {ideaId} GO " +
                          $"DELETE FROM IdeaTargetGroup WHERE IdeaId = {ideaId} GO " +
                          $"DELETE FROM Idea WHERE Id = {ideaId} GO ";
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
            await ctx.Database.ExecuteSqlCommandAsync(command);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
        }

        public async Task<bool> CanCommentIdea(int userId, int ideaId)
        {
            return await ctx.Ideas.AnyAsync(i =>
                i.Id == ideaId &&
                i.TargetGroups.Any(g =>
                    g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
                i.Users.All(u => u.UserId != userId) || (i.CreatorUserId == userId)
            );
        }



        public async Task<IdeaComment> CreateComment(int userId, int ideaId, string textDelta)
        {
            var comment = new IdeaComment
            {
                CreationTime = DateTime.Now,
                IdeaId = ideaId,
                TextDelta = textDelta,
                UserId = userId,
            };
            ctx.IdeaComments.Add(comment);
            await ctx.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> IsCreatorOfComment(int commentId, int userId)
        {
            return await ctx.IdeaComments.AnyAsync(c => c.Id == commentId && c.UserId == userId);
        }

        public async Task DeleteComment(int commentId)
        {
            ctx.IdeaComments.Remove(new IdeaComment { Id = commentId });
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateGotIt(int ideaId, bool gotIt)
        {
            var idea = await ctx.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId);
            if (idea == null)
                throw KariajiException.BadParamters;
            idea.GotIt = gotIt;

        }

        public async Task<bool> CanUpdateTextDeltaOfIdea(int ideaId, int userId)
        {
            return await ctx.Ideas.AnyAsync(i => i.Id == ideaId && i.CreatorUserId == userId);
        }
        public async Task UpdateTextDeltaOfIdea(int ideaId, string textDelta)
        {
            var idea = await ctx.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId);
            idea.TextDelta = textDelta;
            await this.ctx.SaveChangesAsync();
        }

    }
}
