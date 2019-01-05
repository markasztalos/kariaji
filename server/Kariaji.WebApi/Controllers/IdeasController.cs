using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Models;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kariaji.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class IdeasController: KariajiBaseController
    {
        readonly IdeasManagerService ideasSvc;
        public IdeasController(UserGroupManagerService ugSvc, IdeasManagerService ideasSvc) : base(ugSvc)
        {
            this.ideasSvc = ideasSvc;
        }

        [HttpPost]
        [Route("ideas")]
        public async Task<ActionResult<CompactIdeaInfo>> CreateNewIdea([FromBody]CreateIdeaModel model)
        {
            if (!(await this.ugSvc.IsMemberOfGroups(model.TargetGroupIds, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;
            var idea = await this.ideasSvc.CreateNewIdeaAsyn(model, CurrentUser.Id);
            return idea.ToCompactInfo(IsFriendGroup, IsFriendUser);
        }

        [HttpGet]
        [Route("ideas/except-mine")]
        public async Task<ActionResult<List<IdeaInfo>>> GetVisibleIdeasExceptMine()
        {
            return (await this.ideasSvc.GetVisibleIdeasExceptMine(CurrentUser.Id)).Select(i => (i.ToInfo(IsFriendGroup, IsFriendUser)))
                .ToList();
        }

        [HttpGet]
        [Route("ideas")]
        public async Task<ActionResult<List<IdeaInfo>>> GetVisibleIdeas()
        {
            return (await this.ideasSvc.GetVisibleIdeas(CurrentUser.Id)).Select(i => (i.ToInfo(IsFriendGroup, IsFriendUser,
                    i.CreatorUserId == CurrentUser.Id &&
                    i.Users.Any(u => u.UserId == CurrentUser.Id)
                    )))
                .ToList();
        }

        [HttpGet]
        [Route("own-ideas")]
        public async Task<ActionResult<List<CompactIdeaInfo>>> GetOwnIdeas()
        {
            return (await this.ideasSvc.GetOwnIdeas(CurrentUser.Id)).Select(i => i.ToCompactInfo(IsFriendGroup, IsFriendUser))
                .ToList();
        }

        [HttpGet]
        [Route("idea/{ideaId}/reserve")]
        public async Task<ActionResult<ReservationInfo>> Reserve(int ideaId)
        {
            if (!(await this.ideasSvc.CanReserve(CurrentUser.Id, ideaId)))
                throw KariajiException.NotAuthorized;
            var reservation = await this.ideasSvc.Reserve(CurrentUser.Id, ideaId);
            return Ok(reservation.ToInfo());
        }

        [HttpGet]
        [Route("reservation/{reservationId}/join")]
        public async Task<ActionResult> JoinReservation(int reservationId)
        {
            if (!(await this.ideasSvc.CanJoinToReservation(reservationId, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;
            await this.ideasSvc.JoinToReservation(reservationId, CurrentUser.Id);
            return Ok();
        }
        [HttpDelete]
        [Route("reservation/{reservationId}/user/{userId}")]
        public async Task<ActionResult> RemoveJoinReservation(int reservationId, int userId)
        {
            if (!(await this.ideasSvc.CanRemoveJoin(reservationId, CurrentUser.Id, userId)))
                throw KariajiException.NotAuthorized;
            await this.ideasSvc.RemoveJoin(reservationId, userId);
            return Ok();
        }

        [HttpGet]
        [Route("reservation/{reservationId}/update-can-join")]
        public async Task<ActionResult> UpdateIfCanJoinToReservation(int reservationId, bool canJoin)
        {
            if ((await this.ideasSvc.GetReservationCreatorId(reservationId)) != CurrentUser.Id)
                throw KariajiException.NotAuthorized;
            await this.ideasSvc.UpdateIfCanJoinToReservation(reservationId, canJoin);
            return Ok();
        }

        [HttpDelete]
        [Route("reservation/{reservationId}")]
        public async Task<ActionResult> DeleteReservation(int reservationId)
        {
            if ((await this.ideasSvc.GetReservationCreatorId(reservationId)) != CurrentUser.Id)
                throw KariajiException.NotAuthorized;
            await this.ideasSvc.DeleteReservaion(reservationId);
            return Ok();
        }

        [HttpDelete]
        [Route("ideas/{ideaId}")]
        public async Task<ActionResult> DeleteIdea(int ideaId)
        {
            if (!(await ideasSvc.CanDeleteIdea(ideaId, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;
            await this.ideasSvc.DeleteIdea(ideaId);
            return Ok();
        }

        [HttpPost]
        [Route("idea/{ideaId}/comments")]
        public async Task<ActionResult<IdeaCommentInfo>> CreateComment(int ideaId, [FromBody] CreateCommentModel model)
        {
            if (!(await this.ideasSvc.CanCommentIdea(CurrentUser.Id, ideaId)))
                throw KariajiException.NotAuthorized;

            var comment = await this.ideasSvc.CreateComment(CurrentUser.Id, ideaId, model.TextDelta);
            return comment.ToInfo();
        }

        [HttpDelete]
        [Route("comment/{commentId}")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            if (!(await this.ideasSvc.IsCreatorOfComment(commentId, CurrentUser.Id)))
                throw KariajiException.NotAuthorized;

            await this.ideasSvc.DeleteComment(commentId);
            return Ok();
        }

        [HttpGet]
        [Route("idea/{ideaId}/gotIt")]
        public async Task<ActionResult> UpdateGotIt(int ideaId, bool gotIt)
        {
            if (!(await this.ideasSvc.CanReserve(CurrentUser.Id, ideaId)))
                throw KariajiException.NotAuthorized;

            await this.ideasSvc.UpdateGotIt(ideaId, gotIt);
            return Ok();
        }

    }
}
