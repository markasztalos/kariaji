import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { HttpErrorResponse } from '@angular/common/http';
import { NgStoreService } from 'src/app/store/app.store';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';

@Component({
  selector: 'kariaji-password-recovery-page',
  templateUrl: './password-recovery-page.component.html',
  styleUrls: ['./password-recovery-page.component.scss']
})
export class PasswordRecoveryPageComponent implements OnInit {

  constructor(private activatedRoute: ActivatedRoute, 
    private dialogs: KariajiDialogsService,
    
    private router: Router, private authSvc: AuthenticationService, private storeSvc: NgStoreService) { }

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

  async update() {
    await this.authSvc.recoverPassword(this.token, this.password1).subscribe(() => {
      this.dialogs.toastSuccess('A jelszót sikeresen megváltoztattad');
      this.router.navigate(['/login']);
    }, (error: HttpErrorResponse) => {
      alert(error.error.error);
      this.router.navigate(['/register']);
    });
  }

}
