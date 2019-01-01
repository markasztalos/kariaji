import {MatSlideToggleModule} from '@angular/material/slide-toggle';

import {MatButtonToggleModule} from '@angular/material/button-toggle';
import {MatDialogModule} from '@angular/material/dialog';
import {MatListModule} from '@angular/material/list';
import {MatIconModule} from '@angular/material/icon';
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
import { GroupsEditorComponent } from './components/groups-editor/groups-editor.component';
import { MyAccountComponent } from './components/my-account/my-account.component';
import { UpdatePasswordComponent } from './components/update-password/update-password.component';
import { UserGroupApiService } from './services/user-group-adi.service';
import { GroupEditorComponent } from './components/groups-editor/group-editor.component';
import { ConfirmDialogComponent } from './components/common/dialogs/confirm.component';
import { KariajiDialogsService } from './services/dialogs.service';


@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    MainNavComponent,
    LoginPageComponent,
    RegisterPageComponent,
    ConfirmRegistrationComponent,
    SettingsComponent,
    GroupsEditorComponent,
    MyAccountComponent,
    UpdatePasswordComponent,
    GroupEditorComponent,
    ConfirmDialogComponent,
    
  ],
  imports: [
    MatInputModule,
    MatToolbarModule,
    MatSlideToggleModule,
    BrowserModule,
    MatListModule,
    MatSnackBarModule,
    MatButtonToggleModule,
    MatMenuModule,
    FlexLayoutModule,
    MatButtonModule,
    BrowserAnimationsModule,
    MatIconModule,
    AppRoutingModule,
    MatCardModule,
    MatDialogModule,
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
  MyAccountApiService,
  UserGroupApiService,
  KariajiDialogsService

],
  bootstrap: [AppComponent],
  entryComponents: [
    ConfirmDialogComponent
  ]
})
export class AppModule {
  constructor(ngRedux: NgRedux<IKariajiAppState>) {
    ngRedux.provideStore(kariajiReduxStore);
  }
 }
