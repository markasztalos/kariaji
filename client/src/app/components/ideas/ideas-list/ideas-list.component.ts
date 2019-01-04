import { Component, OnInit, ViewChild, TemplateRef, ElementRef } from '@angular/core';
import { CompactIdea, Idea, Group, IdeaComment, CompactUser } from 'src/app/models/models';
import { select } from '@angular-redux/store';
import { IKariajiAppState, IIdeasListState } from 'src/app/store/app.state';
import { Observable } from 'rxjs';
import { IdeasListStateService } from 'src/app/store/ideas-list.redux';
import { IdeasApiService } from 'src/app/services/ideas-api.service';
import { map } from 'rxjs/operators';
import { QuillDeltaToHtmlConverter } from 'quill-delta-to-html';
import { DomSanitizer } from '@angular/platform-browser';
import { ContainerGroupsStateService } from 'src/app/store/container-groups.redux';
import { KariajiDialogsService } from 'src/app/services/dialogs.service';
import { MyAccountStateWrapperService } from 'src/app/store/user-groups.redux';
import { RichTextareaComponent } from '../../common/rich-textarea/rich-textarea.component';

@Component({
  selector: 'kariaji-ideas-list',
  templateUrl: './ideas-list.component.html',
  styleUrls: ['./ideas-list.component.scss']
})
export class IdeasListComponent implements OnInit {

  constructor(private ideasListStateSvc: IdeasListStateService, private ideasApi: IdeasApiService, private myAccountStateSvc: MyAccountStateWrapperService, private sanitizer: DomSanitizer, private cotnainerGroupStateSvc: ContainerGroupsStateService, private dialogs: KariajiDialogsService) { }

  ngOnInit() {
    this.refreshList();
  }

  user$: Observable<CompactUser> = this.myAccountStateSvc.provideCurrentUser();
  userId$: Observable<number> = this.user$.pipe(map(u => u ? u.id : null));

  @select((state: IKariajiAppState) => state.ideasListState)
  ideasListState$: Observable<IIdeasListState>;
  ideas$: Observable<Idea[]> = this.ideasListState$.pipe(map(s => s.ideas));

  refreshList() {
    this.ideasListStateSvc.setShownIdeas(null);
    this.ideasApi.getVisibleIdeas().subscribe(ideas =>
      this.ideasListStateSvc.setShownIdeas(ideas));
  }

  getText(textDelta: string) {
    return this.sanitizer.bypassSecurityTrustHtml(new QuillDeltaToHtmlConverter(JSON.parse(textDelta).ops).convert());
  }

  containerGroups$: Observable<Group[]> = this.cotnainerGroupStateSvc.getContainerGroups$();


  @ViewChild('ideaDetailsDialogTemplate') ideaDetailsDialogTemplate: TemplateRef<any>;
  detailedIdea: Idea;
  showDetailsOfIdea(idea: Idea) {
    this.detailedIdea = idea;
    this.dialogs.matDialogs.open(this.ideaDetailsDialogTemplate);
  }

  deleteComment(idea: Idea, comment: IdeaComment) {
    this.ideasApi.deleteComment(comment.id).subscribe(() =>
      idea.comments.splice(idea.comments.indexOf(comment), 1));

  }


  sendComment() {
    if (this.newCommentTextArea.getText().trim()) {
      const newCommentDelta = this.newCommentTextArea.getContents();
      this.ideasApi.createComment(this.detailedIdea.id, newCommentDelta).subscribe(newComment => {
        if (!this.detailedIdea.comments)
          this.detailedIdea.comments = [];
        this.detailedIdea.comments.push(newComment);
        this.newCommentTextArea.resetText();
      });
    }
  }

  @ViewChild('newCommentTextArea') newCommentTextArea: RichTextareaComponent;
}
