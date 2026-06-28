import { Component, input, signal } from '@angular/core';
import { Tour, TourLog, TransportType } from '../../model/model';
import { OverlayService } from '../../services/OverlayService';
import { TourService } from '../../services/TourService';

@Component({
  selector: 'app-create-log',
  imports: [],
  templateUrl: './create-log.html',
  styleUrl: './create-log.css',
})
export class CreateLog {
  constructor(public tourService:TourService, public overlay:OverlayService){}
    tour=input.required<Tour>();
    
    tourName = signal('');
    difficulty = signal<number | null>(null);
    rating = signal<number | null>(null);
    comment = signal('');
    time = signal<number|null>(null);
    distance = signal<number | null>(null);
    timeString = signal('')
    dateString = signal('')
  
    submit(){
      const timeOfTourString=this.dateString()+"T"+ this.timeString()+":00";
      const timeOfTour=new Date(timeOfTourString);
      timeOfTour.setMonth(timeOfTour.getMonth()+1);
      
      if(this.tourName()!==""&&this.difficulty()&&this.rating()&&this.time()&&this.distance()){
        const newTourLog=new TourLog(-1, this.tourName(), timeOfTour, this.difficulty()??3, this.distance()??0,
        this.time()??0, this.rating()??3, this.comment());
        this.tourService.createTourLog(this.tour().id, newTourLog);
        this.overlay.close();
      }
      else{
        alert("invalid fields");
      }
    }
    parseFloatValue(value: string): number {
      return parseFloat(value);
    }
}
