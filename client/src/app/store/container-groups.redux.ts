import * as Immutable from 'immutable';
import { Injectable } from "@angular/core";
import { UserGroupApiService } from "../services/user-group-adi.service";
import { IKariajiAppState, initialAppState } from "./app.state";
import { NgRedux } from "@angular-redux/store";
import { Group } from "../models/models";
import { createActionWithValue, ActionWithValue } from "./store.common";
import { Reducer } from 'redux';
import { Observable } from 'rxjs';

const SET_CONTAINER_GROUPS = 'container-groups|set';

@Injectable()
export class ContainerGroupsStateService {
    constructor(private ugApi: UserGroupApiService, private ngRedux: NgRedux<IKariajiAppState>) {

    }

    private setContainerGroups(groups: Group[]) {
        this.ngRedux.dispatch(createActionWithValue(SET_CONTAINER_GROUPS, groups));
    }

    public invalidateContainerGroups() {
        this.setContainerGroups(null);
        this.ensureContainerGroups();
    }
    private isContainerGroupsQueryPending = false;
    private ensureContainerGroups() {
        if (!this.ngRedux.getState().__containerGroups && !this.isContainerGroupsQueryPending) {
            this.isContainerGroupsQueryPending = true;
            const query = this.ugApi.getContainerGroups();
            query.subscribe(groups => {
                this.setContainerGroups(groups);
                this.isContainerGroupsQueryPending = false;
            });
        }
    }
    public getContainerGroups$(): Observable<Group[]> {
        this.ensureContainerGroups();
        return this.ngRedux.select(state => state.__containerGroups);
    }

}

export const containerGroupsReducer: Reducer<Group[]> = (state = initialAppState.__containerGroups, action: ActionWithValue<Group[]>) => {
    switch (action.type) {
        case SET_CONTAINER_GROUPS: return action.value;
        default: return state;
    }
};

