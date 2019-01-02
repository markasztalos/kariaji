import { CompactUser } from "../models/models";
import * as Immutable from 'immutable';

export interface IKariajiAppState { 
    __currentUser: CompactUser;
    __currentUserCannotBeQueried : boolean;
    __avatars : Immutable.Map<number, string>;
    __users : Immutable.Map<number, CompactUser>;
}

export const initialAppState : IKariajiAppState = {
    __currentUserCannotBeQueried : false,
    __currentUser : null,
    __avatars : Immutable.Map(),
    __users : Immutable.Map()
};