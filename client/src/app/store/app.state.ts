import { User } from "../models/models";

export interface IKariajiAppState { 
    __currentUser: User;
    __currentUserCannotBeQueried : boolean;
}

export const initialAppState : IKariajiAppState = {
    __currentUserCannotBeQueried : false,
    __currentUser : null
};