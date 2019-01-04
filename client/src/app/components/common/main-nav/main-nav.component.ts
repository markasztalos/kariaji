import { Component, OnInit } from '@angular/core';
import { faGift } from '@fortawesome/free-solid-svg-icons';
import { NewIdeaDialogStateWrapperService } from 'src/app/store/new-idea-dialog.redux';


@Component({
  selector: 'kariaji-main-nav',
  templateUrl: './main-nav.component.html',
  styleUrls: ['./main-nav.component.scss']
})
export class MainNavComponent implements OnInit {

  // icons
  public faGift = faGift;

  constructor(private newIdeaStateSvc : NewIdeaDialogStateWrapperService) { }

  ngOnInit() {
  }
  createNewIdea() {
    this.newIdeaStateSvc.setIsNewDialogShown(true);
  }
}
