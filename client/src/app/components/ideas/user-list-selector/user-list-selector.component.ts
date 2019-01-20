import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'kariaji-user-list-selector',
  templateUrl: './user-list-selector.component.html',
  styleUrls: ['./user-list-selector.component.scss']
})
export class UserListSelectorComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  public setUsersAndSelection(users: number[], selectedUsers: number[]) {
    this._users = users;
    this.selectedUsers = selectedUsers;

  }

  _users: number[] = [];
  get users(): number[] { return this._users; }
  @Input()
  set users(value: number[]) {
    if (value !== this._users) {
      if (value.length === this._users.length) {
        let equal = true;
        for (let i = 0; i < value.length; i++) {
          if (value[i] !== this._users[i]) {
            equal = false;
            break;
          }
        }
        if (equal) {
          return;
        }
      }
      this._users = value;
      const newSelectedUsers = this.selectedUsers ? this.selectedUsers.filter(u => (value ? value : []).indexOf(u) >= 0) : [];
      if (newSelectedUsers.length !== this.selectedUsers.length) {
        this.selectedUsers = newSelectedUsers;
        this.selectedUsersChange.next(this.selectedUsers);
      }
    }

  }

  @Input()
  selectedUsers: number[] = [];

  @Output()
  selectedUsersChange: EventEmitter<number[]> = new EventEmitter();

  @Output()
  userSelected: EventEmitter<number> = new EventEmitter();

  @Output()
  userDeselected: EventEmitter<number> = new EventEmitter();

  @Input()
  cannotRemoveIds: number[] = [];

  get selectableUsers(): number[] {
    return this.users ? this.users.filter(u => this.selectedUsers.indexOf(u) < 0) : [];
  }

  addUser(u: number) {
    this.selectedUsers = [...this.selectedUsers, u];
    this.userSelected.next(u);
    this.selectedUsersChange.next(this.selectedUsers);
  }

  removeUser(u: number) {
    if (this.canDeleteUser(u)) {
      this.selectedUsers = this.selectedUsers.filter(u2 => u2 !== u);
      this.userDeselected.next(u);
      this.selectedUsersChange.next(this.selectedUsers);
    }
  }
  canDeleteUser(uid: number) { return this.cannotRemoveIds.indexOf(uid) < 0; }
  @Input()
  spinning = false;
}
