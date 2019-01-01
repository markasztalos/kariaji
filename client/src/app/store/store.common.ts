import { Action } from "redux";


export interface ActionWithValue<T> extends Action {
    type: string;
    value: T;
};

export function createAction(type: string): Action {
    return { type };
}

export function createActionWithValue<T>(type: string, value: T): ActionWithValue<T> {
    return { type, value };
}

export function extractActionValue<T>(action : Action) {
    return (<ActionWithValue<T>>action).value as T;
}