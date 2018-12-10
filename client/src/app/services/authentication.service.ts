import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService implements CanActivate {

  private apiBaseUrl: string;
  constructor(private http: HttpClient, public router: Router) {
    this.apiBaseUrl = environment.siteBaseUrl + "api";
  }

  public register(email: string): Observable<{ link: string }> {
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
    const obs = this.http.post<{ token: string }>(`${this.apiBaseUrl}/auth/login`, { email, password });
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


