import { Component, computed, input, InputSignal, signal } from '@angular/core';
import { Tour } from '../../model/model';
import { TourService } from '../../services/TourService';

@Component({
  selector: 'app-tour-list-item',
  imports: [],
  templateUrl: './tour-list-item.html',
  styleUrl: './tour-list-item.css',
})
export class TourListItem {
  constructor(public tourService: TourService) {
  }
  math=Math
  tour = input.required<Tour>()
  showDetail=signal(false)  

  isSelected = computed(() => {
    const selected = this.tourService.selectedTour();
    return selected !== null && selected.id === this.tour().id;
  });
  activateTourLogView(){
    this.tourService.tourLogView.update(val=>true);
    queueMicrotask(()=>{
      this.tourService.selectTour(this.tour().id);
    });
  }
  toggleSelection(){
    if(this.isSelected()){
      this.tourService.selectTour("");
    }
    else{
      this.tourService.selectTour(this.tour().id)
    }
  }
}
