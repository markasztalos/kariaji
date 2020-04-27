import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Group, UserGroupInvitation, GroupMember } from 'src/app/models/models';
import { UserGroupApiService } from 'src/app/services/user-group-adi.service';
import { NgRedux } from '@angular-redux/store';
import { IKariajiAppState } from 'src/app/store/app.state';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { yes, cancel, no, yes_warn } from '../common/dialogs/confirm.component';

@Component({
  selector: 'kariaji-group-editor',
  templateUrl: './group-editor.component.html',
  styleUrls: ['./group-editor.component.scss']
})
export class GroupEditorComponent implements OnInit {

  constructor(private route: ActivatedRoute, private ugApi: UserGroupApiService, private myAccountState: MyAccountStateWrapperService, private ngRedux: NgRedux<IKariajiAppState>, private matDialogs: MatDialog,
    private dialogs: KariajiDialogsService) {
    route.paramMap.subscribe(p => {
      this.groupId = parseInt(p.get('id'));
    });
  }

  private _groupId: number;
  get groupId() { return this._groupId; }
  set groupId(value: number) {
    if (value !== this._groupId) {
      this._groupId = value;
      if (value) {
        this.ugApi.getGroup(this.groupId).subscribe(group => {
          this.group = group;
          this.updateInvitations();
        });
      } else {
        this.updateInvitations();

      }
    }
  }
  group: Group;

  updateInvitations() {
    if (this.groupId && this.isAdministrator) {
      this.ugApi.getInvitationsOfGroup(this.groupId).subscribe(invitations => {
        this.invitations = invitations;
      });
    } else this.invitations = null;
  }

  invitations: UserGroupInvitation[];

  get isAdministrator(): boolean {
    return this.group && this.userId && this.group.members.some(m => (m.user.id === this.userId) && m.isAdministrator);
  }

  userId: number;

  ngOnInit() {
    this.myAccountState.getCurrentUser().subscribe(u => {
      // console.log(u);
      this.userId = u.id;
    });
  }

  @ViewChild('inviteDialogTemplate') inviteDialogTemplate: TemplateRef<any>;
  inviteDialog: MatDialogRef<any>;
  showInviteDialog() {
    this.ugApi.getExistingEmailAddresses(this.group.members.map(m => m.user.id)).subscribe(emails => {
      this.existingEmails = (emails ? emails : [])
        .map(e => e.trim().toLowerCase());
      this.inviteDialog = this.matDialogs.open(this.inviteDialogTemplate);
    });
  }

  get inviteEmailDoesNotExist() {
    return this.existingEmails && this.inviteEmail && (this.existingEmails.indexOf(this.inviteEmail.trim().toLowerCase()) < 0);
  }
  get emailAlreadyMember() {
    return this.inviteEmail && this.group.members.find(m => m.user.email.trim().toLowerCase() === this.inviteEmail);
  }
  get emailAlreadyInvited() {
    return this.inviteEmail && this.invitations && this.invitations.some(i => i.invitedEmail.toLowerCase() == this.inviteEmail.toLowerCase());
  }

  invite() {
    this.ugApi.inviteUserByAddress(this.group.id, this.inviteEmail).subscribe(() => {
      this.inviteDialog.close();
      this.updateInvitations();
      this.dialogs.toastSuccess("Meghívó kiküldve");
    });
  }

  inviteEmail: string;
  existingEmails: string[];

  deleteInvitation(inv: UserGroupInvitation) {
    this.dialogs.showDialog({
      title: "Megerősítés",
      message: "Biztosan törölni szeretnéd ezt a meghívót?",
      buttons: [yes, no]
    }).afterClosed().subscribe(action => {
      if (action === yes.action) {
        this.ugApi.deleteInvitation(inv.id).subscribe(() => {
          this.dialogs.toastSuccess(`A(z) ${inv.invitedEmail} címre kiküldött meghívót töröltük`);
          this.updateInvitations();
        });
      }
    });
  }

  toggleAdmin(m: GroupMember) {

    this.ugApi.updateMembership(m.user.id, this.groupId, !m.isAdministrator).subscribe(() =>
      m.isAdministrator = !m.isAdministrator);
  }
  deleteMember(m: GroupMember) {
    this.dialogs.showDialog({
      title: 'Megerősítés',
      message: 'Biztosan törölni szeretnéd?',
      buttons: [yes_warn, no]
    }).afterClosed().subscribe(result => {
      if (result === yes.action)
        this.ugApi.deleteMembership(m.user.id, this.groupId).subscribe(() =>
          this.group.members.splice(this.group.members.indexOf(m), 1));
    });
  }
}
