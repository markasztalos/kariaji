import { Component, OnInit } from '@angular/core';
import { select } from '@angular-redux/store';
import { MyAccountStateWrapperService, MyAccountActions } from 'src/app/store/user-groups.redux';
import { MyAccountApiService } from 'src/app/services/my-account.service';

@Component({
  selector: 'kariaji-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  constructor(private myAccountStateSvc: MyAccountStateWrapperService, private myAccountApi: MyAccountApiService, private myAccountActions : MyAccountActions) { }

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
    }).subscribe(u => this.myAccountActions.setCurrentUser(u))
  }

}
