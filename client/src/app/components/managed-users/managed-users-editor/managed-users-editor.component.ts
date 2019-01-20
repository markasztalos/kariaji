import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { MatDialogRef, MatTable } from '@angular/material';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { UserGroupApiService } from 'src/app/services/user-group-adi.service';
import { IManagedUserData } from 'src/app/models/models';
import { IKariajiAppState } from 'src/app/store/app.state';
import { NgRedux } from '@angular-redux/store';
import { MyAccountStateWrapperService, UsersStateService } from 'src/app/store/user-groups.redux';
import { ContainerGroupsStateService } from 'src/app/store/container-groups.redux';
import { yes, no } from '../../common/dialogs/confirm.component';




@Component({
  selector: 'kariaji-managed-users-editor',
  templateUrl: './managed-users-editor.component.html',
  styleUrls: ['./managed-users-editor.component.scss']
})
export class ManagedUsersEditorComponent implements OnInit {

  constructor(private dialogs: KariajiDialogsService, private myAccountStateSvc: MyAccountStateWrapperService, private ugApiSvc: UserGroupApiService, private ngRedux: NgRedux<IKariajiAppState>, private usersStateSvc: UsersStateService,
    private groupsStateSvc: ContainerGroupsStateService) { }

  ngOnInit() {
    this.myAccountStateSvc.getCurrentUser().subscribe(u => this.userId = u ? u.id : null);
    this.groupsStateSvc.getContainerGroups$().subscribe(groups => {
      this.containerGroupIds = groups ? groups.map(g => g.id) : [];
      this.users = groups ? groups.mapMany(g => g.members.map(m => m.user.id)).distinct() : [];
    });
    setTimeout(() => this.refresh());

  }

  userId: number;

  users: number[];
  containerGroupIds: number[];

  refresh() {
    this.managedUsers = null;
    this.ugApiSvc.getManagedUsers().subscribe(users => {
      this.managedUsers = users;
      // console.log(this.managedUsers);
      this.managedUsersTable.renderRows();
    });
  }

  @ViewChild("managedUsersTable") managedUsersTable: MatTable<IManagedUserData>;

  managedUsers: IManagedUserData[] = null;
  displayedColumns = ['displayName', 'managerUserIds', 'containerGroupIds', 'actions'];


  newManagedUserDialog: MatDialogRef<any>;
  @ViewChild('newManagedUserDialogTemplate') newManagedUserDialogTemplate: TemplateRef<any>;
  @ViewChild('editManagedUserDialogTemplate') editManagedUserDialogTemplate: TemplateRef<any>;
  showNewManagedUserDialog() {
    this.newManagedUserDialog = this.dialogs.matDialogs.open(this.newManagedUserDialogTemplate);
    this.newUserDiaplayName = '';
  }
  newUserDiaplayName: string;
  get isNewDisplayNameUsed() {
    return this.newUserDiaplayName && this.managedUsers.some(u => u.displayName.toLowerCase() === this.newUserDiaplayName.trim().toLowerCase());
  }
  get isEditedDisplayNameUsed() {
    return this.editedDisplayName && this.managedUsers.some(u => (u.id !== this.editedManagedUser.id) && u.displayName.toLowerCase() === this.editedDisplayName.trim().toLowerCase());
  }
  createNewManagedUser() {
    this.ugApiSvc.createManagedUser(this.newUserDiaplayName).subscribe(user => {
      this.newUserDiaplayName = '';
      this.newManagedUserDialog.close();
      this.managedUsers.push({
        id: user.id,
        displayName: user.displayName,
        managerUserIds: [this.userId],
        containerGroupIds: [],
      });
      this.managedUsersTable.renderRows();
    });
  }

  editedManagedUser: IManagedUserData;

  updateEditedDisplayName() {
    this.ugApiSvc.updateManagedAccount(this.editedManagedUser.id, this.editedDisplayName).subscribe(() => {
      this.editedManagedUser.displayName = this.editedDisplayName;
      this.usersStateSvc.invlidateUser(this.editedManagedUser.id);
    });
  }

  showEditManagedUserDialog(managedUser: IManagedUserData) {
    this.editedManagedUser = managedUser;
    this.editedDisplayName = managedUser.displayName;
    this.editManagedUserDialog = this.dialogs.matDialogs.open(this.editManagedUserDialogTemplate);
  }

  editedDisplayName: string;

  onManagerUserAddedToEditedManagedUser(userId: number) {
    this.editedUsersSpinning = true;
    this.ugApiSvc.addManagerToUser(userId, this.editedManagedUser.id).subscribe(() => {
      this.editedManagedUser.managerUserIds = [...this.editedManagedUser.managerUserIds, userId];
      this.editedUsersSpinning = false;
    }, err => {
      this.editedManagedUser.managerUserIds = [...this.editedManagedUser.managerUserIds];
      this.editedUsersSpinning = false;
    });
  }
  onManagerUserRemovedFromEditedManagedUser(userId: number) {
    this.editedUsersSpinning = true;

    this.ugApiSvc.removeManagerOfUser(userId, this.editedManagedUser.id).subscribe(() => {
      this.editedManagedUser.managerUserIds = this.editedManagedUser.managerUserIds.filter(u => u !== userId);
      this.editedUsersSpinning = false;

    }, err => {
      this.editedManagedUser.managerUserIds = [...this.editedManagedUser.managerUserIds];
      this.editedUsersSpinning = false;
    });
  }
  onGroupAddedToEditedManagedUser(groupId: number) {
    this.editedGroupsSpinning = true;

    this.ugApiSvc.addManagedUserToGroup(this.editedManagedUser.id, groupId).subscribe(() => {
      this.editedManagedUser.containerGroupIds = [...this.editedManagedUser.containerGroupIds, groupId];
      this.editedGroupsSpinning = false;

    }, () => {
      this.editedManagedUser.containerGroupIds = [...this.editedManagedUser.containerGroupIds];
      this.editedGroupsSpinning = false;
    });
  }

  onGroupRemovedFromEditedManagedUser(groupId: number) {
    this.editedGroupsSpinning = true;

    this.ugApiSvc.removeManagedUserFromGroup(this.editedManagedUser.id, groupId).subscribe(() => {
      this.editedManagedUser.containerGroupIds = this.editedManagedUser.containerGroupIds.filter(u => u !== groupId);
      this.editedGroupsSpinning = false;

    }, () => {
      this.editedManagedUser.containerGroupIds = [...this.editedManagedUser.containerGroupIds];
      this.editedGroupsSpinning = false;
    });
  }

  editManagedUserDialog: MatDialogRef<any>;

  deleteEditedMangedUser() {
    this.dialogs.showDialog({
      title: "Megerősítés",
      message: "Biztosan törölni szeretnéd ezt a fiókot?",
      buttons: [yes, no]
    }).afterClosed().subscribe(action => {
      if (action === yes.action) {
        this.ugApiSvc.deleteManagedUser(this.editedManagedUser.id).subscribe(() => {
          this.editManagedUserDialog.close();
          this.managedUsers = this.managedUsers.filter(u => u.id !== this.editedManagedUser.id);
          this.editedManagedUser = null;
        });
      }
    });
  }
  editedGroupsSpinning: boolean = false;
  editedUsersSpinning: boolean = false;
}
