
import { Component, OnInit, Inject } from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material';

export interface IDialogConfig {
    title : string;
    message : string;
    buttons : IButton[];
}

export interface IButton {
  color?: 'basic' | 'accent' | 'primary' | 'warn';
  label: string;
  action?: string;
}

export const yes: IButton = { color: 'primary', action: 'yes', label: "Igen" };
export const yes_warn: IButton = { color: 'warn', action: 'yes', label: "Igen" };
export const no_accent: IButton = { color: 'accent', action: 'no', label: "Nem" };
export const no: IButton = { color: 'basic', action: 'no', label: "Nem" };
export const cancel: IButton = { color: 'basic', action: 'cancel', label: "Vissza" };
export const ok: IButton = { color: 'primary', action: 'ok', label: "Renben" };

@Component({
  selector: 'kariaji-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.scss']
})
export class DialogComponent implements OnInit {


  title: string;
  message: string;
  customButtons: IButton[] = [];

  constructor(@Inject(MAT_DIALOG_DATA) public data: IDialogConfig) {
    this.title = data.title;
    this.message = data.message;
    this.customButtons = data.buttons ? data.buttons : [];
   }

  ngOnInit() {
  }

}
