import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { NgStoreService } from 'src/app/store/app.store';
import { Router } from '@angular/router';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonResult } from 'src/app/models/common-results';

@Component({
  selector: 'kariaji-forgot-password-page',
  templateUrl: './forgot-password-page.component.html',
  styleUrls: ['./forgot-password-page.component.scss']
})
export class ForgotPasswordPageComponent implements OnInit {

  constructor(private storeSvc: NgStoreService,
    private router: Router,
    private dialogs: KariajiDialogsService,
    private authSvc: AuthenticationService) { }
  
    email: string = '';

  ngOnInit() {
    this.storeSvc.resetAppState();
    this.authSvc.resetToken();

  }

  link: string;

  async send() {
    this.link = null;
    // const result = 
    this.authSvc.requestPassworRecovery(this.email).subscribe(() => {
      this.router.navigate(['/login']);
      this.dialogs.toastSuccess('Az új jelszóról nemsokára emailt fogsz kapni');
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
