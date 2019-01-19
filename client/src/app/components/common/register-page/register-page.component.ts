import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { NgStoreService } from 'src/app/store/app.store';
import { Router } from '@angular/router';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonResult } from 'src/app/models/common-results';

@Component({
  selector: 'kariaji-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent implements OnInit {

  constructor(private storeSvc: NgStoreService,
    private router: Router,
    private dialogs: KariajiDialogsService,
    private authSvc: AuthenticationService) { }
  email1: string = '';
  email2: string = '';

  ngOnInit() {
    this.storeSvc.resetAppState();
    this.authSvc.resetToken();

  }

  link: string;

  async register() {
    this.link = null;
    // const result = 
    this.authSvc.register(this.email1).subscribe(() => {
      this.router.navigate(['/login']);
      this.dialogs.toastSuccess('A sikeres regisztrációról nemsokára emailt fogsz kapni');
    }, (err: HttpErrorResponse) => {
      if (err.error) {
        const result = err.error as CommonResult;
        if (result.message) {
          this.dialogs.toastError(result.message);
        }
      }
    });
    // this.link = result.link;

  }



}
