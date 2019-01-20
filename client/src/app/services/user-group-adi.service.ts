import { Injectable } from "@angular/core";
import { ApiBaseService, buildUrl } from "./api-base-service";
import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { OwnMembership, CompactGroup, Group, UserGroupInvitation, CompactUser, FriendsData, IManagedUserData } from "../models/models";
import { Observable } from "rxjs";
import { KariajiDialogsService } from "./dialogs.service";

@Injectable()
export class UserGroupApiService extends ApiBaseService {
    deleteInvitation(id: number): Observable<any> {
        return this.delete(`/invitations/${id}`);
    }
    acceptInvitation(id: number): Observable<any> {
        return this.get(`/invitations/${id}/accept`);
    }
    rejectInvitation(id: number): Observable<any> {
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

    constructor(http: HttpClient, dialogs: KariajiDialogsService) {
        super(http, dialogs);
    }

    public getOwnMemberships(): Observable<OwnMembership[]> {
        return this.get<OwnMembership[]>(`/memberships`);
    }

    getAvatar(userId: number): Observable<Blob> {
        return this.getBlob(`user/${userId}/avatar`);
    }

    getUser(userId: number): Observable<CompactUser> {
        return this.get<CompactUser>(`user/${userId}`);
    }

    updateMembership(userId: number, groupId: number, isAdministrator: boolean): Observable<any> {
        return this.put(`memberships`, { userId, groupId, isAdministrator });
    }
    deleteMembership(userId: number, groupId: number): Observable<any> {
        return this.delete(buildUrl('memberships', { userId, groupId }));
    }
    getContainerGroups(): Observable<Group[]> {
        return this.get(`container-groups`);
    }

    public getDataOfFriends(): Observable<FriendsData> {
        return this.get(`friends`);
    }

    public getManagedUsers(): Observable<IManagedUserData[]> {
        return this.get(`managed-users`);
    }


    public createManagedUser(displayName: string): Observable<CompactUser> {
        return this.post(`managed-users`, { displayName });
    }
    public deleteManagedUser(managedUserId: number) {
        return this.delete(`managed-users/${managedUserId}`);
    }
    public addManagerToUser(managerUserId: number, managedUserId: number) {
        return this.post(`managed-users/${managedUserId}/managers`, { managerUserId });
    }
    public removeManagerOfUser(managerUserId: number, managedUserId: number) {
        return this.delete(`managed-users/${managedUserId}/managers/${managerUserId}`);
    }
    public addManagedUserToGroup(managedUserId: number, groupId: number) {
        return this.put(`managed-users/${managedUserId}/groups/${groupId}`, { groupId });
    }
    public updateManagedAccount(userId: number, displayName: string) {
        return this.put(`users/${userId}`, {
            displayName
        });
    }

    public updateAvatarOfManagedUser(userId: number, file: File): Observable<any> {
        const formData = new FormData();
        formData.append('image', file);
        return this.put(`managed-users/${userId}/avatar`, formData);

    }
    public deleteAvatarOfManagedUser(userId: number): Observable<any> {
        return this.delete(`managed-users/${userId}/avatar`);
    }
    public removeManagedUserFromGroup(managedUserId: number, groupId: number) {
        return this.delete(`managed-users/${managedUserId}/groups/${groupId}`);
    }
}