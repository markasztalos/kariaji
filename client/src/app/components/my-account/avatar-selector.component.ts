import { Component, OnInit, OnChanges, SimpleChanges, ViewChild, ElementRef, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { ok, no, yes } from '../common/dialogs/confirm.component';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';
import { CompactUser } from 'src/app/models/models';
import { Observable, Subject } from 'rxjs';
import { IKariajiAppState } from 'src/app/store/app.state';
import { map, takeUntil } from 'rxjs/operators';
import { UserGroupApiService } from 'src/app/services/user-group-adi.service';
import { MyAccountApiService } from 'src/app/services/my-account.service';
import { AvatarsStateService } from 'src/app/store/avatars.redux';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

const MAX_AVATAR_SIZE = 1 * 1024 * 1024;

@Component({
  selector: 'kariaji-avatar-selector',
  templateUrl: './avatar-selector.component.html',
  styleUrls: ['./avatar-selector.component.scss']
})
export class AvatarSelectorComponent implements OnInit, OnDestroy {

  ngUnsubsribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubsribe.next();
    this.ngUnsubsribe.complete();
  }

  constructor(private sanitizer: DomSanitizer, private myAccountState: MyAccountStateWrapperService, private dialogs: KariajiDialogsService, private ugApi: UserGroupApiService, private myAccountApi: MyAccountApiService, private avatarStateSvc: AvatarsStateService) {

  }

  async ngOnInit() {
    this.userId = (await this.myAccountState.getCurrentUser().toPromise()).id;
    this.displayName$.pipe(takeUntil(this.ngUnsubsribe)).subscribe(() => this.manuallyRefreshAvatarComponent());

    this.avatarStateSvc.getAvatarUrl$(this.userId).pipe(takeUntil(this.ngUnsubsribe)).subscribe(src => {
      this.src = src ? this.sanitizer.bypassSecurityTrustResourceUrl(src) : null;
      this.manuallyRefreshAvatarComponent();
    });

  }

  @ViewChild("fileSelector") fileSelector: ElementRef;
  onClicked() {
    this.fileSelector.nativeElement.click();
  }
  onFileSelected(event) {
    if (event.target.files && event.target.files.length) {
      const file: File = event.target.files[0];
      if (file.size >= MAX_AVATAR_SIZE) {
        this.dialogs.showDialog({
          title: "Hiba",
          message: "A kép mérete nem lehet nagyobb, mint 1 MB",
          buttons: [ok]
        });
        return;
      }

      // const reader = new FileReader();
      // reader.onload = (e: Event) => this.src = (<any>e.target).result;
      // reader.readAsDataURL(file);
      // this.refreshAvatarComponent();
      this.myAccountApi.updateOwnAvatar(file).subscribe(() => {
        this.fileSelector.nativeElement.value = null;
        this.avatarStateSvc.invalidateAvatarUrl(this.userId);
      });
    }
  }

  deleteAvatar() {
    this.myAccountApi.deleteOwnAvatar().subscribe(() =>
      this.avatarStateSvc.invalidateAvatarUrl(this.userId));
  }

  src: SafeResourceUrl;

  showAvatar: boolean = true;
  get showAvatarCopy(): boolean { return !this.showAvatar; }
  manuallyRefreshAvatarComponent() {
    this.showAvatar = false;
    setTimeout(() => this.showAvatar = true);
  }

  userId: number;
  displayName$: Observable<string> = this.myAccountState.provideCurrentUser().pipe(map(u => u ? u.displayName : null));


}
