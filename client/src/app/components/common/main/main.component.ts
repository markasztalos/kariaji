import { Component, OnInit } from '@angular/core';


@Component({
  selector: 'kariaji-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {

  constructor() { }
  ngOnInit() {
    console.log(this.faGift);
  }

}
