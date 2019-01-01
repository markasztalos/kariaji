import { Component, OnInit } from '@angular/core';
import { MyAccountStateWrapperService, MyAccountActions } from 'src/app/store/user-groups.redux';
import { MyAccountApiService } from 'src/app/services/my-account.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'kariaji-my-account',
  templateUrl: './my-account.component.html',
  styleUrls: ['./my-account.component.scss']
})
export class MyAccountComponent implements OnInit {

  constructor(private myAccountStateSvc: MyAccountStateWrapperService, private myAccountApi: MyAccountApiService, private myAccountActions : MyAccountActions, private snackBar: MatSnackBar) { }

  ngOnInit() {
    this.myAccountStateSvc.getCurrentUser().subscribe(u => {
      if (u) {
        this.displayName = u.displayName;
        this.email = u.email;
      }

    });
  }

  displayName: string;
  email : string;

  updateMyAccount() {
    this.myAccountApi.updateMyAccount({
      displayName: this.displayName
    }).subscribe(u => {
      this.myAccountActions.setCurrentUser(u);
      this.snackBar.open("Felhasználói fiók frissítve", "Ok");
    });
  }

}
