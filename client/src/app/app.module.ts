
import {MatTableModule} from '@angular/material/table';
import {MatExpansionModule} from '@angular/material/expansion';

import {MatSelectModule} from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import {MatCheckboxModule} from '@angular/material/checkbox';

import {MatTooltipModule} from '@angular/material/tooltip';

import {MatBadgeModule} from '@angular/material/badge';


import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { ToastrModule } from 'ngx-toastr';
import { AvatarModule } from 'ngx-avatar';

import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDialogModule } from '@angular/material/dialog';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { NgReduxModule, NgRedux } from '@angular-redux/store';
import { MatToolbarModule } from '@angular/material/toolbar';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';

import { MatButtonModule } from '@angular/material/button';
import { FlexLayoutModule } from '@angular/flex-layout';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

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
import { kariajiReduxStore, NgStoreService } from './store/app.store';
import { MyAccountActions, MyAccountStateWrapperService, UsersStateService } from './store/user-groups.redux';
import { SettingsComponent } from './components/settings/settings.component';
import { MyAccountApiService } from './services/my-account.service';
import { GroupsEditorComponent } from './components/groups-editor/groups-editor.component';
import { MyAccountComponent } from './components/my-account/my-account.component';
import { UpdatePasswordComponent } from './components/update-password/update-password.component';
import { UserGroupApiService } from './services/user-group-adi.service';
import { GroupEditorComponent } from './components/groups-editor/group-editor.component';
import { DialogComponent } from './components/common/dialogs/confirm.component';
import { KariajiDialogsService } from './services/dialogs.service';
import { AvatarSelectorComponent } from './components/my-account/avatar-selector.component';
import { AvatarsActions, AvatarsStateService, NewIdeaDialogStateWrapperService, IdeasListStateService } from './store/kariaji.store.public';
import { UserAvatarComponent } from './components/common/user-avatar/user-avatar.component';
import { MylistComponent } from './components/mylist/mylist.component';
import { GiftsComponent } from './components/gifts/gifts.component';
import { RichTextareaComponent } from './components/common/rich-textarea/rich-textarea.component';
import { NewIdeaComponent } from './components/mylist/idea-editor/new-idea.component';
import { ContainerGroupsStateService } from './store/container-groups.redux';
import { IdeasApiService } from './services/ideas-api.service';
import { IdeasListComponent } from './components/ideas/ideas-list/ideas-list.component';
import { GroupAvatarComponent } from './components/common/group-avatar/group-avatar.component';
import { EditIdeaComponent } from './components/ideas/edit-idea/edit-idea.component';
import { GroupListSelectorComponent } from './components/ideas/group-list-selector/group-list-selector.component';
import { UserListSelectorComponent } from './components/ideas/user-list-selector/user-list-selector.component';
import { FriendsService } from './services/friends.service';
import { ManagedUsersEditorComponent } from './components/managed-users/managed-users-editor/managed-users-editor.component';
import { DocComponent } from './components/common/doc.component';
import { ForgotPasswordPageComponent } from './components/common/forgot-password-page/forgot-password-page.component';
import { PasswordRecoveryPageComponent } from './components/common/password-recovery-page/password-recovery-page.component';



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
    DialogComponent,
    AvatarSelectorComponent,
    UserAvatarComponent,
    MylistComponent,
    GiftsComponent,
    RichTextareaComponent,
    NewIdeaComponent,
    IdeasListComponent,
    GroupAvatarComponent,
    EditIdeaComponent,
    GroupListSelectorComponent,
    UserListSelectorComponent,
    ManagedUsersEditorComponent,
    DocComponent,
    ForgotPasswordPageComponent,
    PasswordRecoveryPageComponent

  ],
  imports: [
    MatInputModule,
    MatToolbarModule,
    MatBadgeModule,
    MatSlideToggleModule,
    MatCheckboxModule,
    BrowserModule,
    MatProgressSpinnerModule,
    ToastrModule.forRoot(),
    MatListModule,
    MatSnackBarModule,
    MatButtonToggleModule,
    MatMenuModule,
    AvatarModule,
    FlexLayoutModule,
    MatButtonModule,
    BrowserAnimationsModule,
    MatIconModule,
    AppRoutingModule,
    MatCardModule,
    MatDialogModule,
    FontAwesomeModule,
    MatSelectModule,
    MatTooltipModule,
    MatTableModule,
    MatExpansionModule,
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
    AvatarsActions,
    FriendsService,
    KariajiDialogsService,
    UsersStateService,
    NgStoreService,
    AvatarsStateService,
    ContainerGroupsStateService,
    NewIdeaDialogStateWrapperService,
    IdeasApiService,
    IdeasListStateService
  ],
  bootstrap: [AppComponent],
  entryComponents: [
    DialogComponent
  ]
})
export class AppModule {
  constructor(ngRedux: NgRedux<IKariajiAppState>) {
    ngRedux.provideStore(kariajiReduxStore);
  }
}
