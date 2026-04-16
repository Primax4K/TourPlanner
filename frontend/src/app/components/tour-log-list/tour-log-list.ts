import { Component, input } from '@angular/core';
import { Tour } from '../../model/model';
import { TourService } from '../../services/TourService';
import { TourLogListItem } from '../tour-log-list-item/tour-log-list-item';
@Component({
  selector: 'app-tour-log-list',
  imports: [TourLogListItem],
  templateUrl: './tour-log-list.html',
  styleUrl: './tour-log-list.css',
})
export class TourLogList {
  constructor(public tourService:TourService){
    console.log(tourService.tours());
  }
  tour=input.required<Tour>();
  back(){
    this.tourService.tourLogView.update(val=>val=false)
  }
}
