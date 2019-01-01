import { Injectable } from "@angular/core";
import { MatDialog, MatDialogRef } from "@angular/material";
import { IConfirmDialogConfig, ConfirmDialogComponent } from "../components/common/dialogs/confirm.component";

@Injectable()
export class KariajiDialogsService {
    constructor(private matDialogs: MatDialog) {

    }

    confirm(config : IConfirmDialogConfig) : MatDialogRef<any> {
        return this.matDialogs.open(
            ConfirmDialogComponent,
            {
                data: config
            }
        );
    }

}