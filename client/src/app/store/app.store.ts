import { IKariajiAppState, initialAppState } from "./app.state";
import { createLogger } from 'redux-logger';
import { Store, createStore, applyMiddleware, Action, combineReducers, Reducer } from "redux";
import { currentUserReducer, currentUserCannotBeQueriedReducer, usersReducer } from "./user-groups.redux";
import { composeWithDevTools } from 'redux-devtools-extension';
import { avatarsReducer } from "./avatars.redux";
import { Injectable } from "@angular/core";
import { NgRedux } from "@angular-redux/store";
import { createAction } from "./store.common";
import { containerGroupsReducer } from "./container-groups.redux";

const rootReducer: Reducer<IKariajiAppState> = combineReducers<IKariajiAppState>({
    __currentUser: currentUserReducer,
    __currentUserCannotBeQueried: currentUserCannotBeQueriedReducer,
    __avatars: avatarsReducer,
    __users: usersReducer,
    __containerGroups: containerGroupsReducer
});

const RESET_STORE = "store|reset";

export const appReducer: Reducer<IKariajiAppState> = (state: IKariajiAppState = initialAppState, action: Action) => {
    switch (action.type) {
        case RESET_STORE: return {...initialAppState};
    }
    return rootReducer(state, action);
};

export const kariajiReduxStore: Store<IKariajiAppState> = createStore(
    appReducer,
    composeWithDevTools(/*applyMiddleware(createLogger())*/)
);

@Injectable()
export class NgStoreService {
    constructor(private ngRedux : NgRedux<IKariajiAppState>) {}
    public resetAppState(){
        this.ngRedux.dispatch(createAction(RESET_STORE));
    }

}



