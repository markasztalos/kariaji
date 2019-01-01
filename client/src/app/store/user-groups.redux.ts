import { Injectable } from "@angular/core";
import { CompactUser } from "../models/models";
import { IKariajiAppState, initialAppState } from "./app.state";
import { NgRedux } from "@angular-redux/store";
import { Reducer, Action } from "redux";
import { createActionWithValue, extractActionValue, ActionWithValue } from "./store.common";
import { Observable, of } from "rxjs";
import { MyAccountApiService } from "../services/my-account.service";
import { filter, map, first } from "rxjs/operators";

const SET_CURRENT_USER = "current-user/set";
const SET_CURRENT_USER_CANNOT_BE_QUERIED = "current-user-cannot-be-queried/set";

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