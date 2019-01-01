import { Injectable } from "@angular/core";
import { ApiBaseService } from "./api-base-service";
import { HttpClient } from "@angular/common/http";
import { CompactUser, UpdateMyAccountModel } from "../models/models";
import { Observable } from "rxjs";
import { MatSnackBar } from "@angular/material/snack-bar";

@Injectable()
export class MyAccountApiService extends ApiBaseService {
    constructor(http: HttpClient, snackBar: MatSnackBar) { super(http, snackBar); }

    getMyAccount(): Observable<CompactUser> {
        return this.get<CompactUser>(`/my-account`, { silentError: true });
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
}