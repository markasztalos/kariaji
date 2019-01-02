
import * as Immutable from 'immutable';
import { Reducer } from 'redux';
import { initialAppState, IKariajiAppState } from './app.state';
import { ActionWithValue, createActionWithValue } from './store.common';
import { Injectable } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { UserGroupApiService } from '../services/user-group-adi.service';
import { Observable } from 'rxjs';

const SET_AVATAR_URL = 'avatar|set';
const INVALIDATE_AVATAR_URL = 'avatar|invalidate';

export const avatarsReducer: Reducer<Immutable.Map<number, string>> = (state: Immutable.Map<number, string> = initialAppState.__avatars, action: ActionWithValue<{ userId: number, url?: string }>) => {
    switch (action.type) {
        case SET_AVATAR_URL: return state.set(action.value.userId, action.value.url);
        case INVALIDATE_AVATAR_URL: return state.delete(action.value.userId);
        default: return state;
    }
}


@Injectable()
export class AvatarsActions {
    constructor(private ngRedux: NgRedux<IKariajiAppState>) { }

    public setAvatarUrl(userId: number, url: string) {
        this.ngRedux.dispatch(createActionWithValue(SET_AVATAR_URL, { userId, url }));
    }
    public invalidateAvatarUrl(userId: number) {
        this.ngRedux.dispatch(createActionWithValue(INVALIDATE_AVATAR_URL, { userId }));
    }
}

@Injectable()
export class AvatarsStateService {
    constructor(private ugApi: UserGroupApiService, private ngRedux: NgRedux<IKariajiAppState>, private avatarActions: AvatarsActions) {

    }
    public invalidateAvatarUrl(userId: number) {
        this.avatarActions.invalidateAvatarUrl(userId);
        this.ensureAvatarUrl(userId);
    }

    private pendingAvatarQueries: Set<number> = new Set();
    public getAvatarUrl$(userId: number): Observable<string> {
        this.ensureAvatarUrl(userId);
        return this.ngRedux.select(state => state.__avatars.get(userId));
    }
    ensureAvatarUrl(userId: number) {
        if (!this.ngRedux.getState().__avatars.has(userId) && !this.pendingAvatarQueries.has(userId)) {
            const query = this.ugApi.getAvatar(userId);
            this.pendingAvatarQueries.add(userId);
            query.subscribe(blob => {
                this.pendingAvatarQueries.delete(userId);
                if (blob) {
                    const localUlr = URL.createObjectURL(blob);
                    this.avatarActions.setAvatarUrl(userId, localUlr);
                } else {
                    this.avatarActions.setAvatarUrl(userId, null);
                }
            });
        }
    }
}