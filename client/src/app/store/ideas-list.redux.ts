import { Injectable } from "@angular/core";
import { NgRedux } from "@angular-redux/store";
import { IKariajiAppState, IIdeasListState, initialAppState } from "./app.state";
import { Idea } from "../models/models";
import { createActionWithValue, extractActionValue } from "./store.common";
import { Reducer, Action } from "redux";

const SET_IDEAS = 'ideas-list/ideas|state';

@Injectable()
export class IdeasListStateService {
    constructor(private ngRedudx: NgRedux<IKariajiAppState>) {

    }

    public setShownIdeas(ideas: Idea[]) {
        this.ngRedudx.dispatch(createActionWithValue(SET_IDEAS, ideas));
    }

    
}

export const ideasListReducer: Reducer<IIdeasListState> = (state = initialAppState.ideasListState, action: Action) => {
    switch (action.type) {
        case SET_IDEAS: return { ...state, 
            ideas: extractActionValue<Idea[]>(action) }
        default: return state;
    }
}