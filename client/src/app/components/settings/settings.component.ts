import { Component, OnInit } from '@angular/core';
import { select } from '@angular-redux/store';
import { MyAccountStateWrapperService, MyAccountActions } from 'src/app/store/user-groups.redux';
import { MyAccountApiService } from 'src/app/services/my-account.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'kariaji-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {

  

}
