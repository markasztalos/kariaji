import { Component, OnInit } from '@angular/core';
import { select } from '@angular-redux/store';
import { MyAccountStateWrapperService, MyAccountActions, UsersStateService } from 'src/app/store/user-groups.redux';
import { MyAccountApiService } from 'src/app/services/my-account.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { map } from 'rxjs/operators';

@Component({
  selector: 'kariaji-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  ngOnInit(): void {

  }

  constructor(private myAccountStateSvc: MyAccountStateWrapperService) {

  }
  userId$ = this.myAccountStateSvc.provideCurrentUser().pipe(map(user => user && user.id));

}
