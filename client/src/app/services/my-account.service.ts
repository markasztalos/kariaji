import { Injectable } from "@angular/core";
import { ApiBaseService } from "./api-base-service";
import { HttpClient } from "@angular/common/http";
import { User, UpdateMyAccountModel } from "../models/models";
import { Observable } from "rxjs";
import { MatSnackBar } from "@angular/material/snack-bar";

@Injectable()
export class MyAccountApiService extends ApiBaseService {
    constructor(http: HttpClient, snackBar: MatSnackBar) { super(http, snackBar); }

    getMyAccount() : Observable<User> {
        return this.http.get<User>(`${this.apiBaseUrl}/my-account`);
    }

    updateMyAccount(model : UpdateMyAccountModel) : Observable<User> {
        return this.handleError(
            this.http.put<User>(`${this.apiBaseUrl}/my-account`, model));
    }
}