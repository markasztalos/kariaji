import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';


@Component({
  selector: 'kariaji-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {

  constructor(private authSvc : AuthenticationService) { }
  ngOnInit() {

  }

  test() {
    this.authSvc.test();
  }

}
