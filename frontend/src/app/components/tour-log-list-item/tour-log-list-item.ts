import { Component, input } from '@angular/core';
import { TourLog } from '../../model/model';

@Component({
  selector: 'app-tour-log-list-item',
  imports: [],
  templateUrl: './tour-log-list-item.html',
  styleUrl: './tour-log-list-item.css',
})
export class TourLogListItem {
  tourLog=input.required<TourLog>();
}
