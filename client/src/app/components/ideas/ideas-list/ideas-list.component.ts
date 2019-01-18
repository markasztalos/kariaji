import { Component, OnInit, ViewChild, TemplateRef, ElementRef, OnDestroy } from '@angular/core';
import { CompactIdea, Idea, Group, IdeaComment, CompactUser, Reservation } from 'src/app/models/models';
import { select, NgRedux } from '@angular-redux/store';
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
import { MatDialogRef } from '@angular/material';
import { UserGroupApiService } from 'src/app/services/user-group-adi.service';
import { FriendsService } from 'src/app/services/friends.service';
import { UserListSelectorComponent } from '../user-list-selector/user-list-selector.component';

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

  constructor(private ideasListStateSvc: IdeasListStateService,
    private ideasApi: IdeasApiService,
    private myAccountStateSvc: MyAccountStateWrapperService,
    private sanitizer: DomSanitizer,
    private friendsSvc: FriendsService,
    private cotnainerGroupStateSvc: ContainerGroupsStateService,
    private ugApiSvc: UserGroupApiService,
    private ngRedux: NgRedux<IKariajiAppState>,
    private dialogs: KariajiDialogsService) { }

  ngOnInit() {

    this.ensureFriends();

    // this.refreshList();

    this.containerGroups$.pipe(takeUntil(this.ngUnsubscribe)).subscribe(cgs => {
      this.containerGroups = cgs;
      this.filters.groupIds = cgs ? cgs.sort((g1, g2) => g1.displayName.localeCompare(g2.displayName)).map(cg => cg.id) : [];
      this.filters.userIds = cgs ? cgs.mapMany(cg => cg.members.map(m => m.user)).sort((u1, u2) => u1.displayName.localeCompare(u2.displayName)).map(u => u.id).distinct() : [];
      this.filters.onlyNotReserved = false;
      this.filters.onlyReservedByMe = false;
      this.filters.onlySentByMe = false;
      this.filters.skip = 0;
      this.filters.take = 30;

    });


    this.userId$.pipe(takeUntil(this.ngUnsubscribe)).subscribe(uid => this.userId = uid);
  }


  ensureFriends(): any {
    this.friendsSvc.ensureFriends();
  }

  user$: Observable<CompactUser> = this.myAccountStateSvc.provideCurrentUser();
  userId$: Observable<number> = this.user$.pipe(map(u => u ? u.id : null));
  userId: number;

  @select((state: IKariajiAppState) => state.ideasListState)
  ideasListState$: Observable<IIdeasListState>;
  ideas$: Observable<Idea[]> = this.ideasListState$.pipe(map(s => s.ideas));

  refreshList() {

    this.ideasListStateSvc.setShownIdeas(null);
    this.filters.skip = 0;

    this.runQuery();
  }

  runQuery() {
    this.showSpinner = true;
    this.ideasListStateSvc.setShownIdeas(null);
    this.ideasApi.getVisibleIdeas(this.filters.groupIds, this.filters.userIds, this.filters.onlyNotReserved, this.filters.onlyReservedByMe, this.filters.onlySentByMe, this.filters.skip, this.filters.take).subscribe(result => {
      this.ideasListStateSvc.setShownIdeas(result.ideas);
      this.showSpinner = false;
      this.hasMore = result.hasMore;
      this.count = result.ideas.length;
    }, () => this.showSpinner = false);
  }

  showSpinner: boolean;

  getText(textDelta: string) {
    return this.sanitizer.bypassSecurityTrustHtml(new QuillDeltaToHtmlConverter(JSON.parse(textDelta).ops).convert());
  }

  containerGroups$: Observable<Group[]> = this.cotnainerGroupStateSvc.getContainerGroups$();
  containerGroupIds$: Observable<number[]> = this.containerGroups$.pipe(map(gs => gs ? gs.sort((g1, g2) => g1.displayName.localeCompare(g2.displayName)).map(g => g.id) : []));
  containerGroups: Group[];

  get userIdsInFilteredGroups(): number[] {
    return this.containerGroups ? this.containerGroups.filter(cg => this.filters.groupIds.indexOf(cg.id) >= 0).mapMany(g => g.members.map(m => m.user)).sort((u1, u2) => u1.displayName.localeCompare(u2.displayName)).map(u => u.id).distinct() : [];
  }

  get usersInAllContainerGroups(): number[] {
    return this.containerGroups ? this.containerGroups.mapMany(cg => cg.members.map(m => m.user)).sort((a, b) => a.displayName.localeCompare(b.displayName)).map(u => u.id).distinct() : [];
  }

  get page() {
    return Math.ceil(this.filters.skip / this.filters.take);
  }
  hasMore: boolean = false;

  get take() { return this.filters.take; }
  set take(value: string | number) {
    this.filters.take = (typeof value === 'string') ? parseInt(value) : value;
    this.refreshList();
  }

  filters: {
    groupIds: number[],
    userIds: number[],
    onlyNotReserved: boolean,
    onlyReservedByMe: boolean,
    onlySentByMe: boolean,
    take: number;
    skip: number;
  } = {
      groupIds: [],
      userIds: [],
      onlyNotReserved: false,
      onlyReservedByMe: false,
      onlySentByMe: false,
      take: 30,
      skip: 0
    };



  deleteComment(idea: Idea, comment: IdeaComment) {
    this.ideasApi.deleteComment(comment.id).subscribe(() =>
      idea.comments.splice(idea.comments.indexOf(comment), 1));

  }




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
  @ViewChild('newIdeaDialogTemplate') newIdeaDialogTemplate: TemplateRef<any>;
  editIdeaDialog: MatDialogRef<any>;
  detailedIdea: Idea;
  showDetailsOfIdea(idea: Idea) {
    this.detailedIdea = idea;
    this.editIdeaDialog = this.dialogs.matDialogs.open(this.editIdeaDialogTemplate, { width: '800px' });
  }
  closeEditIdeaDialog() {
    this.editIdeaDialog.close();
    this.editIdeaDialog = null;
    this.detailedIdea = null;
  }


  @ViewChild('editIdeaDialogTemplate') editIdeaDialogTemplate: TemplateRef<any>;
  newIdeaDialog: MatDialogRef<any>;
  showCreateNewIdea() {
    this.newIdeaDialog = this.dialogs.matDialogs.open(this.newIdeaDialogTemplate, { width: '800px' });
  }
  closeNewIdeaDialog() {
    this.newIdeaDialog.close();
    this.newIdeaDialog = null;
  }

  navigateToFirstPage() {
    this.filters.skip = 0;
    this.runQuery();
  }
  navigateToNextPage() {
    this.filters.skip += this.filters.take;
    this.runQuery();
  }
  navigateToPreviousPage() {
    this.filters.skip -= this.filters.take;
    this.runQuery();
  }
  get maxIndexOfShownIdeas() {
    // console.log(this.skip);
    // console.log(this.count);
    // console.log(this.skip + this.count);
    return this.filters.skip + this.count;
  }
  count: number = 0;

  showMyList() {
    this.filters.groupIds = this.containerGroups.map(g => g.id);
    this.filters.userIds = [this.userId];
    this.filters.skip = 0;
    this.filters.onlyNotReserved = false;
    this.filters.onlyReservedByMe = false;
    this.filters.onlySentByMe = false;
    this.refreshList();
  }

  @ViewChild('filteredUserIdsSelector') filteredUserIdsSelector : UserListSelectorComponent;

  showUserList(userId: number) {
    this.filters.groupIds = this.containerGroups.filter(cg => cg.members.some(m => m.user.id === userId)).map(g => g.id);
    this.filters.userIds = [userId];
    this.filteredUserIdsSelector.setUsersAndSelection(this.userIdsInFilteredGroups, this.filters.userIds);
    this.filters.skip = 0;
    this.filters.onlyNotReserved = false;
    this.filters.onlyReservedByMe = false;
    this.filters.onlySentByMe = false;
    this.refreshList();
  }
  showAll() {
    this.filters.groupIds = this.containerGroups.map(g => g.id);
    this.filters.userIds = this.userIdsInFilteredGroups;
    this.filters.skip = 0;
    this.filters.onlyNotReserved = false;
    this.filters.onlyReservedByMe = false;
    this.filters.onlySentByMe = false;
    this.refreshList();

  }
}


