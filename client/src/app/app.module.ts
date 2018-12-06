import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './components/common/main/main.component';
import { MainNavComponent } from './components/common/main-nav/main-nav.component';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoginPageComponent } from './components/common/login/login-page.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthenticationInterceptorService } from './services/authentication-interceptor.service';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    MainNavComponent,
    LoginPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FontAwesomeModule,
    HttpClientModule
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: AuthenticationInterceptorService,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
