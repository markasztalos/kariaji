import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { CommonResult } from "../models/common-results";
import { MatSnackBar } from "@angular/material/snack-bar";
import { share, map } from "rxjs/operators";
import { KariajiDialogsService } from "./dialogs.service";

export interface IApiConfig {
    handleError?: boolean;
    silentError?: boolean;
}
const defaultApiConfig: IApiConfig = {
    handleError: true,
    silentError: false
};
export class ApiBaseService {
    protected apiBaseUrl: string;
    constructor(protected http: HttpClient, protected dialogs: KariajiDialogsService) {
        this.apiBaseUrl = environment.siteBaseUrl + "api";
    }

    protected handleError<T>(query: Observable<T>, silent: boolean = false): Observable<T> {
        query.subscribe(null, (error: HttpErrorResponse) => {
            const e = error.error as CommonResult;
            console.log(e);
            if (!silent && e.message) {
                this.dialogs.toastSuccess(e.message);
            }
        });
        return query;
    }

    protected getBlob(urlPart: string, config: IApiConfig = {}): Observable<Blob> {
        
        if (urlPart && urlPart.startsWith("/"))
            urlPart = urlPart.substr(1);
        let o = this.http.get(`${this.apiBaseUrl}/${urlPart}`, {
            responseType: 'blob',
            observe: 'response'
        }).pipe(share());
        const c = { ...{ handleError: true, silentError: true }, ...config };
        if (c.handleError) {
            o = this.handleError(o, c.silentError);
        }
        
        return o.pipe(map(response => response.body));
    }

    protected get<T = any>(urlPart: string, config: IApiConfig = {}): Observable<T> {
        if (urlPart && urlPart.startsWith("/"))
            urlPart = urlPart.substr(1);
        let o = this.http.get<T>(`${this.apiBaseUrl}/${urlPart}`).pipe(share());
        const c = { ...defaultApiConfig, ...config };
        if (c.handleError) {
            o = this.handleError(o, c.silentError);
        }
        return o;
    }
    protected delete<T = any>(urlPart: string, config: IApiConfig = {}): Observable<T> {
        if (urlPart && urlPart.startsWith("/"))
            urlPart = urlPart.substr(1);
        let o = this.http.delete<T>(`${this.apiBaseUrl}/${urlPart}`).pipe(share());
        const c = { ...defaultApiConfig, ...config };
        if (c.handleError) {
            o = this.handleError(o, c.silentError);
        }
        return o;
    }
    protected post<T = any>(urlPart: string, data: any = {}, config: IApiConfig = {}): Observable<T> {
        if (urlPart && urlPart.startsWith("/"))
            urlPart = urlPart.substr(1);
        let o = this.http.post<T>(`${this.apiBaseUrl}/${urlPart}`, data).pipe(share());
        const c = { ...defaultApiConfig, ...config };
        if (c.handleError) {
            o = this.handleError(o, c.silentError);
        }
        return o;
    }
    protected put<T = any>(urlPart: string, data: any, config: IApiConfig = {}): Observable<T> {
        if (urlPart && urlPart.startsWith("/"))
            urlPart = urlPart.substr(1);
        let o = this.http.put<T>(`${this.apiBaseUrl}/${urlPart}`, data).pipe(share());
        const c = { ...defaultApiConfig, ...config };
        if (c.handleError) {
            o = this.handleError(o, c.silentError);
        }
        return o;
    }
}

export function buildUrl(url: string, parameters: any): string {
    if (!url.endsWith("?"))
        url = url + "?";
    for (const prop in parameters) {
        const p = parameters[prop];
        if (p instanceof Array) {
            for (const item of <[]>p)
                url += `&${prop}=${item}`;
        } else {
            url += `&${prop}=${p}`;
        }
    }
    return url;
}