import { IKariajiAppState } from "./app.state";
import { createLogger } from 'redux-logger';
import { Store, createStore, applyMiddleware, Action, combineReducers, Reducer } from "redux";
import { currentUserReducer, currentUserCannotBeQueriedReducer } from "./user-groups.redux";

const rootReducer: Reducer<IKariajiAppState> = combineReducers<IKariajiAppState>({
    __currentUser: currentUserReducer,
    __currentUserCannotBeQueried:  currentUserCannotBeQueriedReducer
}) ;


export const kariajiReduxStore: Store<IKariajiAppState> = createStore(
    rootReducer,
    applyMiddleware(createLogger())
);



