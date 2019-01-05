import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ContainerGroupsStateService } from 'src/app/store/container-groups.redux';
import { Subject, Observable, combineLatest, BehaviorSubject } from 'rxjs';
import { combineReducers } from 'redux';
import { map } from 'rxjs/operators';
import { Group } from 'src/app/models/models';
import { avatarColors } from '../user-avatar/user-avatar.component';



@Component({
  selector: 'kariaji-group-avatar',
  templateUrl: './group-avatar.component.html',
  styleUrls: ['./group-avatar.component.scss']
})
export class GroupAvatarComponent implements OnInit, OnDestroy {


  ngUnsubscribe = new Subject();
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  groupId$: BehaviorSubject<number> = new BehaviorSubject(null);
  @Input()
  public set groupId(value: number) {
    this.groupId$.next(value);
  }


  constructor(private cgStateSvc: ContainerGroupsStateService) { }

  ngOnInit() {
  }

  containerGroups$ = this.cgStateSvc.getContainerGroups$();

  group$: Observable<Group> = combineLatest(
    this.groupId$,
    this.containerGroups$
  ).pipe(map(values => {
    const groupId = values[0];
    const containerGroups = values[1];
    return containerGroups ? containerGroups.find(cg => cg.id === groupId) : null;
  }));
  name$: Observable<string> = this.group$.pipe(map(g => g ? g.displayName : ''));
  description$: Observable<string> = this.group$.pipe(map(g => g ? g.description : ''));

 
  @Input()
  size:number = 35;

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
