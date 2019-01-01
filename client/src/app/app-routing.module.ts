import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginPageComponent } from './components/common/login/login-page.component';
import { MainComponent } from './components/common/main/main.component';
import { RegisterPageComponent } from './components/common/register-page/register-page.component';
import { ConfirmRegistrationComponent } from './components/common/confirm-registration/confirm-registration.component';
import { AuthenticationService } from './services/authentication.service';
import { SettingsComponent } from './components/settings/settings.component';
import { GroupsEditorComponent } from './components/groups-editor/groups-editor.component';
import { GroupEditorComponent } from './components/groups-editor/group-editor.component';

const routes: Routes = [
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  { path: 'confirm-registration', component: ConfirmRegistrationComponent },
  {
    path: '', component: MainComponent,
    canActivate: [AuthenticationService],
    children: [
      { path: 'settings', component: SettingsComponent }
      { path: 'groups', component: GroupsEditorComponent }
      {path : 'group/:id', component: GroupEditorComponent}
    ]
  },
  { path: '**', component: LoginPageComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
