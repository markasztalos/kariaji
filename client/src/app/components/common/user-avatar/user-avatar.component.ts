import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { AvatarsStateService } from 'src/app/store/avatars.redux';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';
import { takeUntil } from 'rxjs/operators';
import { UsersStateService } from 'src/app/store/user-groups.redux';
import { CompactUser } from 'src/app/models/models';

export const avatarColors = [
  "#1abc9c",
  "#3498db",
  "#f1c40f",
  "#8e44ad",
  "#e74c3c",
  "#d35400",
  "#2c3e50",
  "#7f8c8d"
];

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

  constructor(private avatarsState: AvatarsStateService, private sanitizer: DomSanitizer, private usersState: UsersStateService) { }

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
        this.avatarSubscription = this.avatarsState.getAvatarUrl$(value).pipe(takeUntil(this.ngUnsubscribe)).subscribe(src => this.src = src ? this.sanitizer.bypassSecurityTrustResourceUrl(src) : null);
        this.userSubscription = this.usersState.getUser$(value).pipe(takeUntil(this.ngUnsubscribe)).subscribe(u => this.user = u);

      }

    }
  }
  @Input()
  size: number = 35;

  
  showAvatar: boolean = true;

  src: SafeResourceUrl;
  user: CompactUser;

  ngOnInit() {
  }

  public getRandomColor(avatarText: string): string {
    if (!avatarText) {
      return "transparent";
    }
    const asciiCodeSum = this.calculateAsciiCode(avatarText);
    return avatarColors[asciiCodeSum % avatarColors.length];
  }
  private calculateAsciiCode(value: string): number {
    return value
      .split("")
      .map(letter => letter.charCodeAt(0))
      .reduce((previous, current) => previous + current);
  }

}
