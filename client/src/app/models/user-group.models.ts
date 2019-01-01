export interface CompactUser {
    displayName : string;
    email : string;
    id : number;
}

export class UpdateMyAccountModel {
    displayName : string;
}

export interface CompactGroup {
    displayName : string;
    id : number;
    creationDate : string;
    creatorUserDisplayName : string;
    description : string;
    
}
export interface Group extends CompactGroup {
    members : GroupMember[];
}

export class GroupMember {
    user : CompactUser;
    isAdministrator : boolean;
}

export class OwnMembership {
    groupId : number;
    groupDisplayName : string;
    groupDescription : string;
    isAdministrator : boolean;    
    
}

export class UserGroupInvitation {
    id : number;
    groupId : number;
    groupDisplayName : string;
    invitedUser : CompactUser;
    senderUser : CompactUser;
    invitedEmail : string;
    sendingDate : string;
}