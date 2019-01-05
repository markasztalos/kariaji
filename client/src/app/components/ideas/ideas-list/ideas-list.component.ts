import { Component, OnInit, ViewChild, TemplateRef, ElementRef, OnDestroy } from '@angular/core';
import { CompactIdea, Idea, Group, IdeaComment, CompactUser, Reservation } from 'src/app/models/models';
import { select } from '@angular-redux/store';
import { IKariajiAppState, IIdeasListState } from 'src/app/store/app.state';
import { Observable, Subject } from 'rxjs';
import { IdeasListStateService } from 'src/app/store/ideas-list.redux';
import { IdeasApiService } from 'src/app/services/ideas-api.service';
import { map, takeUntil } from 'rxjs/operators';
import { QuillDeltaToHtmlConverter } from 'quill-delta-to-html';
import { DomSanitizer } from '@angular/platform-browser';
import { ContainerGroupsStateService } from 'src/app/store/container-groups.redux';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';
import { RichTextareaComponent } from '../../common/rich-textarea/rich-textarea.component';

@Component({
  selector: 'kariaji-ideas-list',
  templateUrl: './ideas-list.component.html',
  styleUrls: ['./ideas-list.component.scss']
})
export class IdeasListComponent implements OnInit, OnDestroy {

  private ngUnsubscribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  constructor(private ideasListStateSvc: IdeasListStateService, private ideasApi: IdeasApiService, private myAccountStateSvc: MyAccountStateWrapperService, private sanitizer: DomSanitizer, private cotnainerGroupStateSvc: ContainerGroupsStateService, private dialogs: KariajiDialogsService) { }

  ngOnInit() {
    this.refreshList();

    this.userId$.pipe(takeUntil(this.ngUnsubscribe)).subscribe(uid => this.userId = uid);
  }

  user$: Observable<CompactUser> = this.myAccountStateSvc.provideCurrentUser();
  userId$: Observable<number> = this.user$.pipe(map(u => u ? u.id : null));
  userId: number;

  @select((state: IKariajiAppState) => state.ideasListState)
  ideasListState$: Observable<IIdeasListState>;
  ideas$: Observable<Idea[]> = this.ideasListState$.pipe(map(s => s.ideas));

  refreshList() {
    this.ideasListStateSvc.setShownIdeas(null);
    this.ideasApi.getVisibleIdeas().subscribe(ideas =>
      this.ideasListStateSvc.setShownIdeas(ideas));
  }

  getText(textDelta: string) {
    return this.sanitizer.bypassSecurityTrustHtml(new QuillDeltaToHtmlConverter(JSON.parse(textDelta).ops).convert());
  }

  containerGroups$: Observable<Group[]> = this.cotnainerGroupStateSvc.getContainerGroups$();


  @ViewChild('ideaDetailsDialogTemplate') ideaDetailsDialogTemplate: TemplateRef<any>;
  detailedIdea: Idea;
  showDetailsOfIdea(idea: Idea) {
    this.detailedIdea = idea;
    this.dialogs.matDialogs.open(this.ideaDetailsDialogTemplate);
  }

  deleteComment(idea: Idea, comment: IdeaComment) {
    this.ideasApi.deleteComment(comment.id).subscribe(() =>
      idea.comments.splice(idea.comments.indexOf(comment), 1));

  }


  sendComment() {
    if (this.newCommentTextArea.getText().trim()) {
      const newCommentDelta = this.newCommentTextArea.getContents();
      this.ideasApi.createComment(this.detailedIdea.id, newCommentDelta).subscribe(newComment => {
        if (!this.detailedIdea.comments)
          this.detailedIdea.comments = [];
        this.detailedIdea.comments.push(newComment);
        this.newCommentTextArea.resetText();
      });
    }
  }

  @ViewChild('newCommentTextArea') newCommentTextArea: RichTextareaComponent;

  // getReservationButtonText(idea: Idea): string {
  //   if (!idea.isReserved) {
  //     return "Foglalás";
  //   } else {
  //     if (idea.reservation && idea.reservation.reserverUserId === this.userId) {
  //       return "Lefoglaltam, foglalás törlése törlése";
  //     } else {
  //       return "Már lefoglalták";
  //     }
  //   }
  // }
  isReservationButtonDisabled(idea: Idea): boolean {
    return idea.isReserved && (!idea.reservation || idea.reservation.reserverUserId !== this.userId);
  }
  canReserve(idea: Idea): boolean {
    return ((idea.targetUserIds.indexOf(this.userId) < 0) && (idea.secretUserIds.indexOf(this.userId) < 0));
  }
  reserve(idea: Idea) {
    this.ideasApi.reserve(idea.id).subscribe((res) => {
      idea.isReserved = true;
      idea.reservation = res;
    });
  }
  deleteReservation(idea: Idea) {
    this.ideasApi.deleteReservation(idea.reservation.id).subscribe(() => {
      idea.isReserved = false;
      idea.reservation = null;
    });
  }

  updateCanJoin(reservation: Reservation) {

    this.ideasApi.updateIfCanJoinToReservation(reservation.id, !reservation.canJoin).subscribe(() => {
      reservation.canJoin = !reservation.canJoin;
    });
  }

  getTextIfCanJoinForOthersReservation(reservation: Reservation) {
    if (reservation.canJoin) {
      return ', még lehet csatlakozni';
    } else if (!reservation.canJoin && reservation.joinedUserIds && reservation.joinedUserIds.length)
      return ', már nem lehet csatlakozni';
    return '';
  }
  isUserJoined(reservation: Reservation) {
    return reservation.joinedUserIds && (reservation.joinedUserIds.indexOf(this.userId) >= 0);
  }
  detailedReservation: Reservation;
  join(reservation: Reservation) {

    this.ideasApi.joinReservation(reservation.id).subscribe(() => {
      if (!reservation.joinedUserIds)
        reservation.joinedUserIds = [];
      reservation.joinedUserIds.push(this.userId);
    });
  }
  removeJoin(reservation: Reservation, userId: number) {
    this.ideasApi.removeJoinReservation(reservation.id, userId).subscribe(() => {
      reservation.joinedUserIds.splice(reservation.joinedUserIds.indexOf(this.userId), 1);
    });
  }

  @ViewChild('joinedUsersDialogTemplate') joinedUsersDialogTemplate: TemplateRef<any>;
  showJoinedUsers(reservation: Reservation) {
    this.detailedReservation = reservation;

    this.dialogs.matDialogs.open(this.joinedUsersDialogTemplate).afterClosed().subscribe(() => {
      this.detailedReservation = null;
    });
  }

}


