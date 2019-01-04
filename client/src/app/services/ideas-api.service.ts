import { Inject, Injectable } from "@angular/core";
import { ApiBaseService } from "./api-base-service";
import { HttpClient } from "@angular/common/http";
import { KariajiDialogsService } from "./dialogs.service";
import { CreateIdeaModel, CompactIdea, Idea, IdeaComment } from "../models/models";
import { Observable } from "rxjs";

@Injectable()
export class IdeasApiService extends ApiBaseService {
    constructor(http: HttpClient, dialogs: KariajiDialogsService) {
        super(http, dialogs);
    }

    public createNewIdea(model: CreateIdeaModel): Observable<CompactIdea> {
        return this.post<CompactIdea>('ideas', model);
    }
    public getVisibleIdeasExceptMine(): Observable<Idea[]> {
        return this.get<Idea[]>('ideas/except-mine');
    }

    public getVisibleIdeas(): Observable<Idea[]> {
        return this.get<Idea[]>('ideas');
    }

    public getOwnIdeas() {
        return this.get<CompactIdea[]>('own-ideas');
    }

    public reserve(ideaId: number) {
        return this.get(`idea/${ideaId}/reserve`);

    }

    public joinReservation(reservationId: number) {
        return this.get(`reservation/${reservationId}/join`);

    }
    public removeJoinReservation(joinId: number) {
        return this.delete(`reservation-joins/${joinId}`);

    }

    public updateIfCanJoinToReservation(reservationId: number, canJoin: boolean) {
        return this.get(`reservation/${reservationId}/update-can-join`);
    }

    public deleteReservation(reservationId: number) {
        return this.delete(`reservation/${reservationId}`);

    }

    public deleteIdea(ideaId: number) {
        return this.delete(`ideas/${ideaId}`);

    }

    public createComment(ideaId : number, textDelta : string) : Observable<IdeaComment> {
        return this.post<IdeaComment>(`idea/${ideaId}/comments`, {textDelta});
    }
    public deleteComment(commentId : number) : Observable<any> {
        return this.delete(`comment/${commentId}`);

    }


}