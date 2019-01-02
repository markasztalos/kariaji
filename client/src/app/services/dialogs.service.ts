import { Injectable } from "@angular/core";
import { ToastrService } from 'ngx-toastr';
import { MatDialog, MatDialogRef, MatSnackBar, MatSnackBarConfig } from "@angular/material";
import { IDialogConfig, DialogComponent } from "../components/common/dialogs/confirm.component";


const toastOptions = {
    closeButton: false,
    progressBar: true,
    positionClass: 'toast-top-center'
};
@Injectable()
export class KariajiDialogsService {
    constructor(public matDialogs: MatDialog, private snackBar : MatSnackBar, public toastr: ToastrService ) {

    }

    showDialog(config : IDialogConfig) : MatDialogRef<any> {
        return this.matDialogs.open(
            DialogComponent,
            {
                data: config
            }
        );
    }



    toastSuccess(message: string, title? : string) {
        // this.snackBar.open(message, action, {
        //     duration: 2000,
        //     horizontalPosition: 'center',
        //     verticalPosition: 'top'
        // } as MatSnackBarConfig);
        this.toastr.success(message, title, toastOptions);
    }

}