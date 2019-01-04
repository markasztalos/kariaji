import { Reducer } from "redux";
import { initialAppState, IKariajiAppState } from "./app.state";
import { ActionWithValue, createActionWithValue } from "./store.common";
import { Injectable } from "@angular/core";
import { NgRedux } from "@angular-redux/store";

const SET_IS_NEW_IDEA_DIALOG_SHOWN = 'isNewIdeaDialogShown|set';

export const isNewIdeaDialogShownReducer : Reducer<boolean> = (state = initialAppState.isNewIdeaDialogShown, action : ActionWithValue<boolean>) => {
    switch (action.type) {
        case SET_IS_NEW_IDEA_DIALOG_SHOWN: return action.value;
        default : return state;
    }
}

@Injectable()
export class NewIdeaDialogStateWrapperService {
    constructor(private ngRedux: NgRedux<IKariajiAppState>) {
        
    }
    public setIsNewDialogShown(isShown : boolean) {
        this.ngRedux.dispatch(createActionWithValue(SET_IS_NEW_IDEA_DIALOG_SHOWN, isShown));
    }
}