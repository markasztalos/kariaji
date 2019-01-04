
export interface CompactIdea {
    id: number;
    creatorUserId: number;
    creationTime: string;

    textDelta: string;

    targetGroupIds: number[];
    targetUserIds: number[];
    secretUserIds: number[];
}

export interface Idea extends CompactIdea {
    reservation: Reservation;
    isReserved : boolean;
    comments: IdeaComment[];
}

export interface IdeaComment {
    id: number;
    creationTime: string;
    textDelta: string;
    userId: number;
}

export interface Reservation {
    id: number;
    reserverUserId: number;
    reservationTime: string;
    canJoin: boolean;
    joinedUserIds: number[];
}



export interface CreateIdeaModel {
    textDelta: string;
    targetGroupIds: number[];
    targetUserIds: number[];
    secretUserIds: number[];
}