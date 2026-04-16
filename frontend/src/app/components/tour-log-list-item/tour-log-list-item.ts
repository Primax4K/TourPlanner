import { Component, computed, input } from '@angular/core';
import { TourLog } from '../../model/model';
import { TourService } from '../../services/TourService';

@Component({
  selector: 'app-tour-log-list-item',
  imports: [],
  templateUrl: './tour-log-list-item.html',
  styleUrl: './tour-log-list-item.css',
})
export class TourLogListItem {
  tourLog=input.required<TourLog>();
  constructor(public tourService:TourService){}
  isSelected=computed(() => {
    const selected = this.tourService.selectedTourLog();
    return selected !== null && selected.id === this.tourLog().id;
  });
  toggleSelection(){
    if(this.isSelected()){
      this.tourService.selectTourLog(-1);
    }
    else{
      this.tourService.selectTourLog(this.tourLog().id)
    }
  }
}
