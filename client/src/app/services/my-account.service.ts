import { Injectable } from "@angular/core";
import { ApiBaseService } from "./api-base-service";
import { HttpClient } from "@angular/common/http";
import { CompactUser, UpdateMyAccountModel } from "../models/models";
import { Observable } from "rxjs";
import { MatSnackBar } from "@angular/material/snack-bar";
import { KariajiDialogsService } from "./dialogs.service";
import { Router } from "@angular/router";

@Injectable()
export class MyAccountApiService extends ApiBaseService {
    constructor(http: HttpClient, dialogs: KariajiDialogsService, private router : Router) { super(http, dialogs); }

    getMyAccount(): Observable<CompactUser> {
        const o = this.get<CompactUser>(`/my-account`, { handleError: false });
        o.subscribe(null, (err) => {
            this.router.navigate(['/login']);
        })
        return o;
    }

    updateMyAccount(model: UpdateMyAccountModel): Observable<CompactUser> {
        return this.put<CompactUser>(`/my-account`, model);
    }

    updatePassword(oldPassword: string, newPassword: string): Observable<any> {
        return this.put(`/my-account/password`, {
            newPassword,
            oldPassword
        });
    }

    
    updateOwnAvatar(file: File) : Observable<any> {
        const formData = new FormData();
        formData.append('image', file);
        return this.put('my-account/avatar', formData);

    }
    deleteOwnAvatar() : Observable<any> {
        return this.delete('my-account/avatar');
    }
}