import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'kariaji-login',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {

  constructor(private authSvc: AuthenticationService, private router : Router) { }

  ngOnInit() {
    this.authSvc.resetToken();
  }

  email : string;
  password : string;
  async login() {
    await this.authSvc.loginAsync(this.email, this.password);
    
  }

}
