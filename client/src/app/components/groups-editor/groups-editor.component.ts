import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { OwnMembership, UserGroupInvitation } from 'src/app/models/models';
import { UserGroupApiService } from 'src/app/services/user-group-adi.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { stringToKeyValue } from '@angular/flex-layout/extended/typings/style/style-transforms';
import { Router } from '@angular/router';

@Component({
  selector: 'kariaji-groups-editor',
  templateUrl: './groups-editor.component.html',
  styleUrls: ['./groups-editor.component.scss']
})
export class GroupsEditorComponent implements OnInit {

  constructor(private ugApi: UserGroupApiService, private dialogs: MatDialog, private snackBar: MatSnackBar, private router : Router) { }

  ngOnInit() {
    this.updateMemberships();
    this.updateInvitations();
  }

  updateInvitations() {
    this.ugApi.getMyInvitations().subscribe(invitations => this.invitations = invitations);
  }

  updateMemberships() {
    this.ugApi.getOwnMemberships().subscribe(ms =>
      this.memberships = (ms ? ms : []).sort((a, b) => a.groupDisplayName.localeCompare(b.groupDisplayName)) );
  }
  editMembership(ms : OwnMembership) {
    this.router.navigateByUrl(`/group/${ms.groupId}`);
  }

  memberships: OwnMembership[];

  @ViewChild('newGroupDialogTemplate') newGroupDialogTemplate: TemplateRef<any>;


  createNewGroupDialog: MatDialogRef<any>;
  showCreateNewGroupDialog() {
    this.ugApi.getExistingGroupNames().subscribe(list => {
      this.existingGroupNames = list.map(item => item.trim().toLowerCase());
      this.createNewGroupDialog = this.dialogs.open(
        this.newGroupDialogTemplate
      );
    });

  }



  get isNameReserved() { return this.existingGroupNames && this.newGroupName && (this.existingGroupNames.find(name => name === this.newGroupName.trim().toLowerCase())); };

  createNewGroup() {
    this.ugApi.createNewGroup(this.newGroupName, this.newGroupDescription).subscribe(() => {
      this.snackBar.open(`'${this.newGroupName}' nevű csoportot létrehoztuk`, "Ok");
      this.newGroupName = '';
      this.newGroupDescription = '';
      this.updateMemberships();
      this.createNewGroupDialog.close();

    });
  }

  existingGroupNames: string[];

  newGroupName: string;
  newGroupDescription: string;

  invitations : UserGroupInvitation[];

  acceptInvitation(inv: UserGroupInvitation) {
      this.ugApi.acceptInvitation(inv.id).subscribe(() => {
        this.updateInvitations();
        this.updateMemberships();
        this.snackBar.open("Hozzáadtunk a csoporthoz");
      });
  }
  rejectInvitation(inv: UserGroupInvitation) {
    this.ugApi.rejectInvitation(inv.id).subscribe(() => {
      this.updateInvitations();
      this.snackBar.open("Meghívó törölve");
    });
}
}
