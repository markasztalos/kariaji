import { CompactUser } from "../models/models";

export interface IKariajiAppState { 
    __currentUser: CompactUser;
    __currentUserCannotBeQueried : boolean;

}

export const initialAppState : IKariajiAppState = {
    __currentUserCannotBeQueried : false,
    __currentUser : null
};