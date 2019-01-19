import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router } from '@angular/router';
import { NgStoreService } from 'src/app/store/kariaji.store.public';

@Component({
  selector: 'kariaji-login',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {

  constructor(private storeSvc : NgStoreService, private authSvc: AuthenticationService, private router : Router) { }

  ngOnInit() {
    this.storeSvc.resetAppState();
    this.authSvc.resetToken();
  }

  email : string;
  password : string;
  async login() {
    await this.authSvc.loginAsync(this.email, this.password);
    
  }

  gotoRegister() {
    this.router.navigate(['/register']);
  }

}
