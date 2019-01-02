import { Component, OnInit, ElementRef, ViewChild, Input } from '@angular/core';
import Quill from 'quill';
import { QuillDeltaToHtmlConverter } from 'quill-delta-to-html';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { setClassOptions } from '@angular-redux/store/lib/src/decorators/helpers';

@Component({
  selector: 'kariaji-rich-textarea',
  templateUrl: './rich-textarea.component.html',
  styleUrls: ['./rich-textarea.component.scss']
})
export class RichTextareaComponent implements OnInit {

  constructor(private sanitizer: DomSanitizer) { }

  @ViewChild('quillEditor') quillEditor: ElementRef;

  quill: Quill;
  ngOnInit() {
    this.quill = new Quill(this.quillEditor.nativeElement, {
      theme: 'snow'
    });
    if (this._tempContent) {
      this.setContent(this._tempContent);
      this._tempContent = null;
    }
  }

  setContent(value: string) {
    this.quill.setContents(
      JSON.parse(value)
    );
  }

  _tempContent: string;
  @Input()
  set content(value: string) {
    if (!this.quill) {
      this._tempContent = value;
    } else {
      this.setContent(value);
    }

  }

  public getContents(): string {
    return JSON.stringify(this.quill.getContents());
  }

  
}
