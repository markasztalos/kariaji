import { IKariajiAppState, initialAppState } from "./app.state";
import { createLogger } from 'redux-logger';
import { Store, createStore, applyMiddleware, Action, combineReducers, Reducer } from "redux";
import { currentUserReducer, currentUserCannotBeQueriedReducer, usersReducer } from "./user-groups.redux";
import { composeWithDevTools } from 'redux-devtools-extension';
import { avatarsReducer } from "./avatars.redux";
import { Injectable } from "@angular/core";
import { NgRedux } from "@angular-redux/store";
import { createAction, ActionWithValue, createActionWithValue } from "./store.common";
import { containerGroupsReducer } from "./container-groups.redux";
import { isNewIdeaDialogShownReducer } from "./new-idea-dialog.redux";
import { ideasListReducer } from "./ideas-list.redux";

const RESET_STORE = "store|reset";
const SET_FRIENDS_QUERIED = 'friendsQueried|set';

const friendsQueriedReducer: Reducer<boolean> = (state: boolean = initialAppState.friendsQueried, action: ActionWithValue<boolean>) => {
    switch (action.type) {
        case SET_FRIENDS_QUERIED:
            return action.value;
        default:
            return state;
    }
}


const rootReducer: Reducer<IKariajiAppState> = combineReducers<IKariajiAppState>({
    __currentUser: currentUserReducer,
    __currentUserCannotBeQueried: currentUserCannotBeQueriedReducer,
    __avatars: avatarsReducer,
    __users: usersReducer,
    __containerGroups: containerGroupsReducer,
    isNewIdeaDialogShown: isNewIdeaDialogShownReducer,
    ideasListState: ideasListReducer,
    friendsQueried: friendsQueriedReducer


});



export const appReducer: Reducer<IKariajiAppState> = (state: IKariajiAppState = initialAppState, action: Action) => {
    switch (action.type) {
        case RESET_STORE: return { ...initialAppState };
    }
    return rootReducer(state, action);
};



export const kariajiReduxStore: Store<IKariajiAppState> = createStore(
    appReducer,
    composeWithDevTools(/*applyMiddleware(createLogger())*/)
);

@Injectable()
export class NgStoreService {
    constructor(private ngRedux: NgRedux<IKariajiAppState>) { }
    public resetAppState() {
        this.ngRedux.dispatch(createAction(RESET_STORE));
    }
    public setFriendsQueried(value: boolean) {
        this.ngRedux.dispatch(createActionWithValue(SET_FRIENDS_QUERIED, value));
    }
}



