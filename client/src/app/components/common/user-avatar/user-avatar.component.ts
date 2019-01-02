import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { AvatarsStateService } from 'src/app/store/avatars.redux';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';
import { takeUntil } from 'rxjs/operators';
import { UsersStateService } from 'src/app/store/user-groups.redux';
import { CompactUser } from 'src/app/models/models';

@Component({
  selector: 'kariaji-user-avatar',
  templateUrl: './user-avatar.component.html',
  styleUrls: ['./user-avatar.component.scss']
})
export class UserAvatarComponent implements OnInit, OnDestroy {

  ngUnsubscribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  constructor(private avatarsState: AvatarsStateService, private sanitizer : DomSanitizer, private usersState : UsersStateService) { }

  userSubscription: Subscription;
  avatarSubscription: Subscription;
  private _userId: number;
  @Input()
  set userId(value: number) {
    if (value !== this._userId) {
      this.src = null;
      this.user = null;
      if (this.userSubscription)
        this.userSubscription.unsubscribe();
      if (this.avatarSubscription)
        this.avatarSubscription.unsubscribe();
      if (value) {
        this.avatarSubscription = this.avatarsState.getAvatarUrl$(value).pipe(takeUntil(this.ngUnsubscribe)).subscribe(src => this.src = src ?  this.sanitizer.bypassSecurityTrustResourceUrl(src) : null);
        this.userSubscription = this.usersState.getUser$(value).pipe(takeUntil(this.ngUnsubscribe)).subscribe(u => this.user = u);

      }

    }
  }
  manuallyRefreshAvatarComponent() {
    this.showAvatar = false;
    setTimeout(() => this.showAvatar = true);
  }
  showAvatar : boolean = true;

  src: SafeResourceUrl;
  user : CompactUser;

  ngOnInit() {
  }

}
