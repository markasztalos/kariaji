import { Component, OnInit, ViewChild, TemplateRef, OnDestroy, Output, EventEmitter } from '@angular/core';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { CompactGroup, CompactUser, Group, CreateIdeaModel } from 'src/app/models/models';
import { Observable, Subject } from 'rxjs';
import { ContainerGroupsStateService } from 'src/app/store/container-groups.redux';
import { takeUntil } from 'rxjs/operators';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';
import { IdeasApiService } from 'src/app/services/ideas-api.service';
import { RichTextareaComponent } from '../../common/rich-textarea/rich-textarea.component';

@Component({
  selector: 'kariaji-new-idea',
  templateUrl: './new-idea.component.html',
  styleUrls: ['./new-idea.component.scss']
})
export class NewIdeaComponent implements OnInit, OnDestroy {
  ngUnsubscribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  constructor(private myAccountState: MyAccountStateWrapperService, private dialogs: KariajiDialogsService, private containerGroupStateSvc: ContainerGroupsStateService, private ideasApi: IdeasApiService) { }

  currentUserId: number;
  async ngOnInit() {
    this.currentUserId = (await this.myAccountState.getCurrentUser().toPromise()).id;
    this.allContainerGroups$.pipe(
      takeUntil(this.ngUnsubscribe)
    ).subscribe(allGroups => {
      this.allContainerGroups = allGroups ? [...allGroups] : [];
      this.groupsAdded = allGroups ? [...allGroups] : [];
      this.usersAdded = [];
      this.addedHiddenUsers = [];
      if (this.addableUsers.length)
        this.usersAdded.push(this.addableUsers.find(u => u.id === this.currentUserId));

    });
  }

  allContainerGroups: Group[] = [];
  allContainerGroups$: Observable<Group[]> = this.containerGroupStateSvc.getContainerGroups$();

  groupsAdded: Group[] = [];
  usersAdded: CompactUser[] = [];

  get addableGroups(): Group[] {
    return this.allContainerGroups ? this.allContainerGroups.filter(g => this.groupsAdded.indexOf(g) < 0) : [];
  }
  get addableUsers(): CompactUser[] {
    return this.groupsAdded.mapMany(g => g.members.map(m => m.user)).distinctBy(u => u.id).filter(u => this.usersAdded.indexOf(u) < 0);
  }

  addedHiddenUsers: CompactUser[] = [];
  get addableHiddenUsers(): CompactUser[] {
    return this.groupsAdded.mapMany(g => g.members.map(m => m.user)).distinctBy(u => u.id).filter(u => this.addedHiddenUsers.indexOf(u) < 0).filter(u => this.usersAdded.indexOf(u) < 0);
  }
  addHiddenUser(u: CompactUser) {
    this.addedHiddenUsers.push(u);
  }
  removeHiddenUser(u: CompactUser) {
    this.addedHiddenUsers.splice(this.addedHiddenUsers.indexOf(u), 1);
  }

  addGroup(g: Group) {
    this.groupsAdded.push(g);
  }
  removeGroup(g: Group) {
    this.groupsAdded.splice(this.groupsAdded.indexOf(g), 1);
    const addableUsers = this.groupsAdded.mapMany(g => g.members.map(m => m.user)).distinctBy(u => u.id);
    this.usersAdded = this.usersAdded.filter(u => addableUsers.indexOf(u) >= 0);
    this.addedHiddenUsers = this.addedHiddenUsers.filter(u => addableUsers.indexOf(u) >= 0);
  }
  addUser(u: CompactUser) {
    this.usersAdded.push(u);
    this.addedHiddenUsers = this.addedHiddenUsers.filter(u2 => this.usersAdded.indexOf(u2) < 0);
  }
  removeUser(u: CompactUser) {
    this.usersAdded.splice(this.usersAdded.indexOf(u), 1);
  }

  addAllGroups() {
    this.groupsAdded = [...this.allContainerGroups];
  }

  setMeAsOnlyTarget() {
    this.usersAdded = [this.addableUsers.find(u => u.id === this.currentUserId)];
    this.addedHiddenUsers = this.addedHiddenUsers.filter(u2 => this.usersAdded.indexOf(u2) < 0);
  }
  get isSetMeAsOnlyTargetVisible() {
    return ((this.usersAdded.length !== 1) || (this.usersAdded[0].id !== this.currentUserId)) && this.addableUsers.find(u => u.id === this.currentUserId);
  }

  @ViewChild('textEditor') textEditor: RichTextareaComponent;

  @Output()
  finished: EventEmitter<any> = new EventEmitter();

  cancel() {
    this.finished.next();
  }
  create() {
    if (this.textEditor.getText().trim()) {
      this.sending = true;
      this.ideasApi.createNewIdea({
        textDelta: this.textEditor.getContents(),
        secretUserIds: this.addedHiddenUsers.map(u => u.id),
        targetGroupIds: this.groupsAdded.map(u => u.id),
        targetUserIds: this.usersAdded.map(u => u.id)
      }).subscribe(() => {
        this.dialogs.toastSuccess('Ötlet elküldve');
        this.finished.next();
      });
    }
  }
  sending: boolean = false;
}
