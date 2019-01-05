import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { select } from '@angular-redux/store';
import { IKariajiAppState, IIdeasListState } from 'src/app/store/app.state';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { Idea, Reservation, IdeaComment, CompactUser } from 'src/app/models/models';
import { IdeasApiService } from 'src/app/services/ideas-api.service';
import { IdeasListStateService } from 'src/app/store/ideas-list.redux';
import { DomSanitizer } from '@angular/platform-browser';
import { RichTextareaComponent } from '../../common/rich-textarea/rich-textarea.component';
import { QuillDeltaToHtmlConverter } from 'quill-delta-to-html';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';

@Component({
  selector: 'kariaji-edit-idea',
  templateUrl: './edit-idea.component.html',
  styleUrls: ['./edit-idea.component.scss']
})
export class EditIdeaComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  constructor(private ideasApi: IdeasApiService, private dialogs: KariajiDialogsService, private ideasStateSvc: IdeasListStateService, private sanitizer: DomSanitizer, private myAccountStateSvc: MyAccountStateWrapperService) { }

  ngOnInit() {
    // this.ideasListState$.pipe(takeUntil(this.ngUnsubscribe)).subscribe(state => {
    //   this.detailedIdea = state.ideas && state.detailedIdeaId ? state.ideas.find(i => i.id === state.detailedIdeaId) : null;
    // });
    this.userId$.pipe(takeUntil(this.ngUnsubscribe)).subscribe(uid => this.userId = uid);
  }

  // @select((state: IKariajiAppState) => state.ideasListState)
  // ideasListState$: Observable<IIdeasListState>;
  @Input()
  detailedIdea: Idea | null = null;

  getText(textDelta: string) {
    return this.sanitizer.bypassSecurityTrustHtml(new QuillDeltaToHtmlConverter(JSON.parse(textDelta).ops).convert());
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

  user$: Observable<CompactUser> = this.myAccountStateSvc.provideCurrentUser();
  userId$: Observable<number> = this.user$.pipe(map(u => u ? u.id : null));
  userId: number;


  updateCanJoin(reservation: Reservation) {

    this.ideasApi.updateIfCanJoinToReservation(reservation.id, !reservation.canJoin).subscribe(() => {
      reservation.canJoin = !reservation.canJoin;
    });
  }

  deleteReservation(idea: Idea) {
    this.ideasApi.deleteReservation(idea.reservation.id).subscribe(() => {
      idea.isReserved = false;
      idea.reservation = null;
    });
  }

  getTextIfCanJoinForOthersReservation(reservation: Reservation) {
    if (reservation.canJoin) {
      return ', még lehet csatlakozni';
    } else if (!reservation.canJoin && reservation.joinedUserIds && reservation.joinedUserIds.length)
      return ', már nem lehet csatlakozni';
    return '';
  }

  removeJoin(reservation: Reservation, userId: number) {
    this.ideasApi.removeJoinReservation(reservation.id, userId).subscribe(() => {
      reservation.joinedUserIds.splice(reservation.joinedUserIds.indexOf(this.userId), 1);
    });
  }

  join(reservation: Reservation) {

    this.ideasApi.joinReservation(reservation.id).subscribe(() => {
      if (!reservation.joinedUserIds)
        reservation.joinedUserIds = [];
      reservation.joinedUserIds.push(this.userId);
    });
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

  cancel() {
    this.ideasStateSvc.setDetailedIdeaId(null);
    this.finished.next();
  }

  @Output()
  public finished = new EventEmitter();

  @ViewChild('textEditor') textEditor: RichTextareaComponent;
  editText() {
    if (!this.textEditor.getText()) {
      this.dialogs.toastError('Kötelező szöveget megadni');
    } else {
      this.ideasApi.updateTextDeltaOfIdea(this.detailedIdea.id, this.detailedIdea.textDelta = this.textEditor.getContents()).subscribe(() => {
        this.dialogs.toastSuccess("Új tartalom elmentve");
      });
    }
  }

}
