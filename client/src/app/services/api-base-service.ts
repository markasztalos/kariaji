import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { CommonResult } from "../models/common-results";
import { MatSnackBar } from "@angular/material/snack-bar";

export class ApiBaseService {
    protected apiBaseUrl: string;
    constructor(protected http: HttpClient, protected snackBar: MatSnackBar) {
        this.apiBaseUrl = environment.siteBaseUrl + "api";
    }

    protected handleError<T>(query: Observable<T>): Observable<T> {
        query.subscribe(null, (error: HttpErrorResponse) => {
            const e = error.error as CommonResult;
            console.log(e);
            if (e.message) {
                this.snackBar.open(e.message, null, {
                    verticalPosition: 'top',
                    duration: 2000,
                });
            }
        });
        return query;
    }
}