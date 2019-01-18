import { CompactUser, Group, Idea } from "../models/models";
import * as Immutable from 'immutable';

export interface IKariajiAppState { 
    __currentUser: CompactUser;
    __currentUserCannotBeQueried : boolean;
    __avatars : Immutable.Map<number, string>;
    __users : Immutable.Map<number, CompactUser>;
    __containerGroups : Group[];
    isNewIdeaDialogShown : boolean;
    ideasListState : IIdeasListState;
    friendsQueried: boolean;
}
export interface IIdeasListState {
    ideas : Idea[];
    detailedIdeaId : number | null;
}

export const initialAppState : IKariajiAppState = {
    __currentUserCannotBeQueried : false,
    __currentUser : null,
    __avatars : Immutable.Map(),
    __users : Immutable.Map(),
    __containerGroups : null,
    isNewIdeaDialogShown : false,
    ideasListState : {
        ideas : null,
        detailedIdeaId : null
    },
    friendsQueried : false
};
