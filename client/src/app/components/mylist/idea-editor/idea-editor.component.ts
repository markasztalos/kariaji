import { Component, OnInit, ViewChild, TemplateRef, OnDestroy } from '@angular/core';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { CompactGroup, CompactUser, Group } from 'src/app/models/models';
import { Observable, Subject } from 'rxjs';
import { ContainerGroupsStateService } from 'src/app/store/container-groups.redux';
import { takeUntil } from 'rxjs/operators';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';

@Component({
  selector: 'kariaji-idea-editor',
  templateUrl: './idea-editor.component.html',
  styleUrls: ['./idea-editor.component.scss']
})
export class IdeaEditorComponent implements OnInit, OnDestroy {
  ngUnsubscribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  constructor(private myAccountState: MyAccountStateWrapperService, private dialogs: KariajiDialogsService, private containerGroupStateSvc: ContainerGroupsStateService) { }

  currentUserId: number;
  async ngOnInit() {
    setTimeout(() => this.showDialog());
    this.currentUserId = (await this.myAccountState.getCurrentUser().toPromise()).id;
    this.allContainerGroups$.pipe(
      takeUntil(this.ngUnsubscribe)
    ).subscribe(allGroups => {
      this.allContainerGroups = allGroups;
      this.groupsAdded = allGroups;
      this.usersAdded = [];
      this.addedHiddenUsers = [];
      if (this.groupsAdded)
        this.usersAdded.push(this.addableUsers.find(u => u.id === this.currentUserId));

    });
  }

  @ViewChild('dialogTemplate') dialogTemplate: TemplateRef<any>;

  showDialog() {
    this.dialogs.matDialogs.open(this.dialogTemplate, {

    }).afterClosed().subscribe(result => {
      if (result) {

      }
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
    return this.groupsAdded.mapMany(g => g.members.map(m => m.user)).distinct().filter(u => this.usersAdded.indexOf(u) < 0);
  }

  addedHiddenUsers: CompactUser[] = [];
  get addableHiddenUsers(): CompactUser[] {
    return this.groupsAdded.mapMany(g => g.members.map(m => m.user)).distinct().filter(u => this.addedHiddenUsers.indexOf(u) < 0).filter(u => this.usersAdded.indexOf(u) < 0);
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
    this.groupsAdded.splice(this.addableGroups.indexOf(g), 1);
    this.usersAdded = this.usersAdded.filter(u => this.addableUsers.indexOf(u) >= 0);
    this.addedHiddenUsers = this.addedHiddenUsers.filter(u => this.addableUsers.indexOf(u) >= 0);
  }
  addUser(u: CompactUser) {
    this.usersAdded.push(u);
    this.addedHiddenUsers = this.addedHiddenUsers.filter(u => this.usersAdded.indexOf(u) < 0);
  }
  removeUser(u: CompactUser) {
    this.usersAdded.splice(this.usersAdded.indexOf(u), 1);
  }


}
