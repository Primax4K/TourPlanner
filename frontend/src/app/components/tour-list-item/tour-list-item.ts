import { Component, input, InputSignal } from '@angular/core';
import { Tour } from '../../model/model';
import { TourService } from '../../services/TourService';

@Component({
  selector: 'app-tour-list-item',
  imports: [],
  templateUrl: './tour-list-item.html',
  styleUrl: './tour-list-item.css',
})
export class TourListItem {
  constructor(public tourService: TourService) {}
  tour = input.required<Tour>()
}
