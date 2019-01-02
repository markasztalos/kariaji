import { Component, OnInit } from '@angular/core';
import { MyAccountApiService } from 'src/app/services/my-account.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';

@Component({
  selector: 'kariaji-update-password',
  templateUrl: './update-password.component.html',
  styleUrls: ['./update-password.component.scss']
})
export class UpdatePasswordComponent implements OnInit {

  constructor(private myAccountApi : MyAccountApiService, private dialogs: KariajiDialogsService) { }

  ngOnInit() {
  }

  oldPassword: string;
  newPassword: string;
  newPassword2: string;

  get isValid() { return this.oldPassword && this.newPassword && (this.newPassword === this.newPassword2); }

  update() {
    this.myAccountApi.updatePassword(this.oldPassword, this.newPassword).subscribe(() => {
      this.dialogs.toastSuccess("Jelszó sikeresen frissítve");
      this.oldPassword = this.newPassword = this.newPassword2 = '';
    }); 
  }

}
