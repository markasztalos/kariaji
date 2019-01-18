import { Injectable } from "@angular/core";
import * as Immutable from 'immutable';

import { CompactUser } from "../models/models";
import { IKariajiAppState, initialAppState } from "./app.state";
import { NgRedux } from "@angular-redux/store";
import { Reducer, Action } from "redux";
import { createActionWithValue, extractActionValue, ActionWithValue } from "./store.common";
import { Observable, of } from "rxjs";
import { MyAccountApiService } from "../services/my-account.service";
import { filter, map, first } from "rxjs/operators";
import { UserGroupApiService } from "../services/user-group-adi.service";

const SET_CURRENT_USER = "current-user|set";
const SET_CURRENT_USER_CANNOT_BE_QUERIED = "current-user-cannot-be-queried|set";

const SET_USERS = 'users|set';
const DELETE_USERS = 'users|delete';

@Injectable()
export class MyAccountActions {
    constructor(private ngRedux: NgRedux<IKariajiAppState>) { }

    public setCurrentUser(user: CompactUser) {
        this.ngRedux.dispatch(createActionWithValue(SET_CURRENT_USER, user));
        if (user)
            this.ngRedux.dispatch(createActionWithValue(SET_CURRENT_USER_CANNOT_BE_QUERIED, true));
    }
    public setCurrentUserCannotBeQueried(value: boolean) {
        this.ngRedux.dispatch(createActionWithValue(SET_CURRENT_USER_CANNOT_BE_QUERIED, value));
    }
}

export const currentUserReducer: Reducer<CompactUser> = (state: CompactUser = initialAppState.__currentUser, action: ActionWithValue<CompactUser>): CompactUser => {
    switch (action.type) {
        case SET_CURRENT_USER: return action.value;
        default: return state;
    }
};

export const currentUserCannotBeQueriedReducer: Reducer<boolean> = (state: boolean = initialAppState.__currentUserCannotBeQueried, action: ActionWithValue<boolean>) => {
    switch (action.type) {
        case SET_CURRENT_USER_CANNOT_BE_QUERIED: return action.value;
        default: return state;
    }
};


@Injectable()
export class MyAccountStateWrapperService {
    constructor(private ngRedux: NgRedux<IKariajiAppState>, private myAccountApiSvc: MyAccountApiService, private myAccountActions: MyAccountActions) { }

    provideCurrentUser(): Observable<CompactUser> {
        this.getCurrentUser();
        // this.ngRedux.select(state => state.__currentUser).subscribe(x=> console.log(x));
        return this.ngRedux.select(state => state.__currentUser);
    }

    private currentUserQuery: Observable<CompactUser>;
    getCurrentUser(): Observable<CompactUser> {
        if (this.ngRedux.getState().__currentUser)
            return of(this.ngRedux.getState().__currentUser);

        if (!this.currentUserQuery) {
            this.currentUserQuery = this.myAccountApiSvc.getMyAccount();
        }

        this.currentUserQuery.subscribe(user => {
            this.myAccountActions.setCurrentUser(user);
            this.myAccountActions.setCurrentUserCannotBeQueried(false);
        },
            err => {
                this.myAccountActions.setCurrentUser(null);
                this.myAccountActions.setCurrentUserCannotBeQueried(true);
            }
        );

        return this.ngRedux.select().pipe(
            filter<IKariajiAppState>(state => (state.__currentUserCannotBeQueried || state.__currentUser) && true),
            map(state => state.__currentUserCannotBeQueried ? null : state.__currentUser),
            first());
    }
}

@Injectable()
export class UsersStateService {
    constructor(private ngRedux: NgRedux<IKariajiAppState>, private ugApi : UserGroupApiService) {

    }
    public setUsersInRedux(users: CompactUser[]) {
        this.ngRedux.dispatch(createActionWithValue(SET_USERS, users));
    }
    private deleteUsersInRedux(userIds: number[]) {
        this.ngRedux.dispatch(createActionWithValue(DELETE_USERS, userIds));
    }
    public invlidateUser(userId: number) {
        this.deleteUsersInRedux([userId]);
        this.ensureUser(userId);
    }
    public getUser$(userId: number) {
        this.ensureUser(userId);
        return this.ngRedux.select(state => state.__users.get(userId));
    }
    private pendingUserQueries : Set<number> = new Set();
    private ensureUser(userId: number) {
        if (!this.ngRedux.getState().__users.has(userId) && !this.pendingUserQueries.has(userId)) {
            this.pendingUserQueries.add(userId);
            const query = this.ugApi.getUser(userId);
            query.subscribe(user => {
                this.pendingUserQueries.delete(userId);
                this.setUsersInRedux([user]);
            });
        }
    }
}



export const usersReducer: Reducer<Immutable.Map<number, CompactUser>> = (state: Immutable.Map<number, CompactUser> = initialAppState.__users, action: ActionWithValue<CompactUser[] | number[]>): Immutable.Map<number, CompactUser> => {
    switch (action.type) {
        case SET_USERS: {
            for (const user of <CompactUser[]>action.value)
                state = state.set(user.id, user);
            return state;
        }
        case DELETE_USERS: {
            for (const userId of <number[]>action.value)
                state = state.delete(userId);
            return state;
        }
        default: return state;
    }
};

