
import { Component, OnInit, Inject } from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material';

export interface IConfirmDialogConfig {
    title : string;
    message : string;
    buttons : IConfirmButton[];
}

export interface IConfirmButton {
  color?: 'basic' | 'accent' | 'primary' | 'warning';
  label: string;
  action?: string;
}

export const yes: IConfirmButton = { color: 'primary', action: 'yes', label: "Igen" };
export const no: IConfirmButton = { color: 'accent', action: 'no', label: "Nem" };
export const cancel: IConfirmButton = { color: 'basic', action: 'cancel', label: "Vissza" };
export const ok: IConfirmButton = { color: 'primary', action: 'ok', label: "Renben" };

@Component({
  selector: 'kariaji-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.scss']
})
export class ConfirmDialogComponent implements OnInit {


  title: string;
  message: string;
  customButtons: IConfirmButton[] = [];

  constructor(@Inject(MAT_DIALOG_DATA) public data: IConfirmDialogConfig) {
    this.title = data.title;
    this.message = data.message;
    this.customButtons = data.buttons ? data.buttons : [];
   }

  ngOnInit() {
  }

}
