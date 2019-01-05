import { Injectable } from "@angular/core";
import { NgRedux } from "@angular-redux/store";
import { IKariajiAppState, IIdeasListState, initialAppState } from "./app.state";
import { Idea } from "../models/models";
import { createActionWithValue, extractActionValue } from "./store.common";
import { Reducer, Action } from "redux";

const SET_IDEAS = 'ideas-list/ideas|set';
const SET_DETAILED_IDEA = 'ideas-list/deatiledIdeaId|set';

@Injectable()
export class IdeasListStateService {
    constructor(private ngRedudx: NgRedux<IKariajiAppState>) {

    }

    public setShownIdeas(ideas: Idea[]) {
        this.ngRedudx.dispatch(createActionWithValue(SET_IDEAS, ideas));
    }
    public setDetailedIdeaId(ideaId: number | null) {
        this.ngRedudx.dispatch(createActionWithValue(SET_DETAILED_IDEA, ideaId));
    }


}

export const ideasListReducer: Reducer<IIdeasListState> = (state = initialAppState.ideasListState, action: Action) => {
    switch (action.type) {
        case SET_IDEAS: return {
            ...state,
            ideas: extractActionValue<Idea[]>(action),
            detailedIdeaId: null
        };
        case SET_DETAILED_IDEA: return {
            ...state,
            detailedIdeaId: extractActionValue<number | null>(action)
        };
        default: return state;
    }
}