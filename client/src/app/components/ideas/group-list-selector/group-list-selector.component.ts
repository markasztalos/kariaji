import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Group } from 'src/app/models/models';

@Component({
  selector: 'kariaji-group-list-selector',
  templateUrl: './group-list-selector.component.html',
  styleUrls: ['./group-list-selector.component.scss']
})
export class GroupListSelectorComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  @Input()
  groups: number[];

  @Input()
  selectedGroups: number[] = [];

  @Output()
  selectedGroupsChange: EventEmitter<number[]> = new EventEmitter();

  @Output()
  groupSelected: EventEmitter<number> = new EventEmitter();

  @Output()
  groupDeselected: EventEmitter<number> = new EventEmitter();

  get selectableGroups(): number[] {
    return this.groups ? this.groups.filter(g => this.selectedGroups.indexOf(g) < 0) : [];
  }

  addGroup(g: number) {
    this.selectedGroups = [...this.selectedGroups,g];
    this.groupSelected.next(g);
    this.selectedGroupsChange.next(this.selectedGroups);
  }

  removeGroup(g: number) {
    this.selectedGroups = this.selectedGroups.filter(g2 => g2 !== g);
    this.groupDeselected.next(g);
    this.selectedGroupsChange.next(this.selectedGroups);
  }

  @Input()
  spinning = false;

}
