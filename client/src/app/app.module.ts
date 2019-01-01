import {MatMenuModule} from '@angular/material/menu';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { NgReduxModule, NgRedux } from '@angular-redux/store';
import {MatToolbarModule} from '@angular/material/toolbar';

import {MatCardModule} from '@angular/material/card';
import {MatInputModule} from '@angular/material/input';

import {MatButtonModule} from '@angular/material/button';
import { FlexLayoutModule } from '@angular/flex-layout';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule} from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './components/common/main/main.component';
import { MainNavComponent } from './components/common/main-nav/main-nav.component';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoginPageComponent } from './components/common/login/login-page.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthenticationInterceptorService } from './services/authentication-interceptor.service';
import { RegisterPageComponent } from './components/common/register-page/register-page.component';
import { ConfirmRegistrationComponent } from './components/common/confirm-registration/confirm-registration.component';
import { IKariajiAppState } from './store/app.state';
import { kariajiReduxStore } from './store/app.store';
import { MyAccountActions, MyAccountStateWrapperService } from './store/user-groups.redux';
import { SettingsComponent } from './components/settings/settings.component';
import { MyAccountApiService } from './services/my-account.service';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    MainNavComponent,
    LoginPageComponent,
    RegisterPageComponent,
    ConfirmRegistrationComponent,
    SettingsComponent
  ],
  imports: [
    MatInputModule,
    MatToolbarModule,
    BrowserModule,
    MatSnackBarModule,
    MatMenuModule,
    FlexLayoutModule,
    MatButtonModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    MatCardModule,
    FontAwesomeModule,
    HttpClientModule,
    FormsModule,
    NgReduxModule 
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: AuthenticationInterceptorService,
    multi: true
  },
  MyAccountActions,
  MyAccountStateWrapperService,
  MyAccountApiService

],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(ngRedux: NgRedux<IKariajiAppState>) {
    ngRedux.provideStore(kariajiReduxStore);
  }
 }
