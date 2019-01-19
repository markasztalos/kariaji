import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { NewIdeaDialogStateWrapperService } from 'src/app/store/new-idea-dialog.redux';
import { select } from '@angular-redux/store';
import { IKariajiAppState, IIdeasListState } from 'src/app/store/app.state';
import { Observable } from 'rxjs';
import { distinctUntilChanged, map } from 'rxjs/operators';
import { MatDialogRef } from '@angular/material';


@Component({
  selector: 'kariaji-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {

  constructor(private authSvc: AuthenticationService, private dialogs: KariajiDialogsService, private newIdeaStateSvc : NewIdeaDialogStateWrapperService) { }
  ngOnInit() { 

    this.isNewIdeaDialogShown$.subscribe(isShown => {
      if (isShown && !this.dialog) {
        this.showDialog();
      } else if (!isShown && this.dialog) {
        this.closeDialog();
      }
    });
  }

  @ViewChild('newIdeaTemplate') newIdeaTemplate: TemplateRef<any>;

  @select((state : IKariajiAppState) => state.isNewIdeaDialogShown) isNewIdeaDialogShown$ : Observable<boolean>;

  dialog: MatDialogRef<any>;
  showDialog() {
    // this.dialog = this.dialogs.matDialogs.open(this.newIdeaTemplate, {
    // });
    // this.dialog.afterClosed().subscribe(()=> this.onNewIdeaEditingFinished());
  }
  closeDialog() {
    // this.dialog.close();
    // this.dialog = null;
  }

  @select((state: IKariajiAppState) => state.ideasListState)
  ideasListState$: Observable<IIdeasListState>;
  detailedIdeaId$: Observable<number | null> = this.ideasListState$.pipe(map(state => state.detailedIdeaId));
  showDetailedIdea$ : Observable<boolean> = this.detailedIdeaId$.pipe(map(id => id !== null));

  onNewIdeaEditingFinished() {
    this.newIdeaStateSvc.setIsNewDialogShown(false);
  }

  year = (new Date()).getFullYear();
}
