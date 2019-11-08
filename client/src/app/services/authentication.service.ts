import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CanActivate, Router } from '@angular/router';
import { MyAccountActions } from '../store/kariaji.store.public';
import { ApiBaseService } from './api-base-service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { KariajiDialogsService } from './dialogs.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService extends ApiBaseService implements CanActivate {

  constructor(http: HttpClient, public router: Router, dialogs: KariajiDialogsService) {
    super(http, dialogs);
  }

  
  public requestPassworRecovery(email : string): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/auth/forgot-password`, {
      email
    });
  }
    
  public recoverPassword(token : string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/auth/password-recovery`, {
      token,
      newPassword
    });
  }

  public register(email: string): Observable</*{ link: string }*/any> {
    return this.http.post<{ link: string }>(`${this.apiBaseUrl}/auth/register`, {
      email
    });
  }

  public confirmRegistration(token: string, password: string): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/auth/confirm-registration`, {
      token,
      password
    });
  }

  canActivate(): boolean {
    if (!this.hasToken()) {
      this.router.navigate(['login']);
      return false;
    }
    return true;
  }

  public readToken() {
    return localStorage['authToken'];
  }

  public hasToken() {
    const token = this.readToken();
    return token && true;
  }

  public writeToken(token: string) {
    localStorage['authToken'] = token;
    
  }

  public resetToken() {
    localStorage.removeItem('authToken');
  }

  public loginAsync(email: string, password: string): Observable<any> {
    const obs = this.handleError(
    this.http.post<{ token: string }>(`${this.apiBaseUrl}/auth/login`, { email, password }));

    obs.subscribe(data => {
      this.writeToken(data.token);
      this.router.navigate(['']);
    });
    return obs;
  }

  public test() {
    this.http.get(`${this.apiBaseUrl}/auth/test`, {}).subscribe(a=>{

    });
  }
}


