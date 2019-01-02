import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { HttpErrorResponse } from '@angular/common/http';
import { NgStoreService } from 'src/app/store/app.store';

@Component({
  selector: 'kariaji-confirm-registration',
  templateUrl: './confirm-registration.component.html',
  styleUrls: ['./confirm-registration.component.scss']
})
export class ConfirmRegistrationComponent implements OnInit {

  constructor(private activatedRoute: ActivatedRoute, private router: Router, private authSvc: AuthenticationService, private storeSvc : NgStoreService) { }

  token: string;
  ngOnInit() {
    this.storeSvc.resetAppState();
    this.authSvc.resetToken();

    this.activatedRoute.queryParams.subscribe(async params => {
      this.token = params['token'];
    });
  }

  password1: string;
  password2: string;

  async confirm() {
    await this.authSvc.confirmRegistration(this.token, this.password1).subscribe(() => {
      this.router.navigate(['/login']);
    }, (error: HttpErrorResponse) => {
      alert(error.error.error);
      this.router.navigate(['/register']);
    });
  }

}
