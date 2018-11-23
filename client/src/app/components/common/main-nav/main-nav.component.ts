import { Component, OnInit } from '@angular/core';
import { faGift } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'kariaji-main-nav',
  templateUrl: './main-nav.component.html',
  styleUrls: ['./main-nav.component.scss']
})
export class MainNavComponent implements OnInit {

  // icons
  public faGift = faGift;

  constructor() { }

  ngOnInit() {
  }

}
