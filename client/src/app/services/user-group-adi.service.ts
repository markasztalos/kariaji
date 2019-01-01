import { Injectable } from "@angular/core";
import { ApiBaseService, buildUrl } from "./api-base-service";
import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { OwnMembership, CompactGroup, Group, UserGroupInvitation } from "../models/models";
import { Observable } from "rxjs";

@Injectable()
export class UserGroupApiService extends ApiBaseService {
    deleteInvitation(id: number): Observable<any> {
        return this.delete(`/invitations/${id}`);
    }
    acceptInvitation(id : number) : Observable<any> {
        return this.get(`/invitations/${id}/accept`);
    }
    rejectInvitation(id : number) : Observable<any> {
        return this.get(`/invitations/${id}/reject`);
    }
    
    getInvitationsOfGroup(groupId: number): Observable<UserGroupInvitation[]> {
        return this.get<UserGroupInvitation[]>(`groups/${groupId}/invitations`);
    }
    
    getMyInvitations(): Observable<UserGroupInvitation[]> {
        return this.get<UserGroupInvitation[]>(`invitations`);
    }

    inviteUserByAddress(groupId: number, email: string): Observable<any> {
        return this.post('/invitations', { groupId, email });
    }
    getExistingEmailAddresses(exceptUserIds: number[]): Observable<string[]> {
        return this.get<string[]>(buildUrl("/users/emails", { exceptUserIds }));
    }
    getGroup(groupId: number): Observable<Group> {
        return this.get<Group>(`/groups/${groupId}`);
    }
    createNewGroup(newGroupName: string, newGroupDescription: string): Observable<CompactGroup> {
        return this.post<CompactGroup>('/groups', {
            name: newGroupName,
            description: newGroupDescription
        });

    }

    getExistingGroupNames(): Observable<string[]> {
        return this.get<string[]>(`/groups/names`);
    }

    constructor(http: HttpClient, snackBar: MatSnackBar) {
        super(http, snackBar);
    }

    public getOwnMemberships(): Observable<OwnMembership[]> {
        return this.get<OwnMembership[]>(`/memberships`);
    }
}