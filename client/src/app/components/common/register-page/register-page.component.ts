import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'kariaji-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent implements OnInit {

  constructor(private authSvc : AuthenticationService) { }
  email1: string = '';
  email2: string = '';

  ngOnInit() {
    this.authSvc.resetToken();
  }

  link : string;

  async register() {
    this.link = null;
    const result = await this.authSvc.register(this.email1).toPromise();
    this.link = result.link;
  }

  

}
