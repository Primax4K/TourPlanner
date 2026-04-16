import { Component, computed, input, OnInit, signal } from '@angular/core';
import { Tour, TourLog } from '../../model/model';
import { TourService } from '../../services/TourService';
import { OverlayService } from '../../services/OverlayService';

@Component({
  selector: 'app-edit-log',
  imports: [],
  templateUrl: './edit-log.html',
  styleUrl: './edit-log.css',
})
export class EditLog implements OnInit{
  constructor(public tourService:TourService, public overlay:OverlayService){}
  tourLog=input.required<TourLog>();
  tour=input.required<Tour>();
  
  difficulty = signal<number | null>(null);
  rating = signal<number | null>(null);
  tourName = signal('');
  timeString = signal('')
  dateString = signal('')
  comment = signal('');
  time = signal<number|null>(null);
  distance = signal<number | null>(null);

  ngOnInit(){
    this.timeString.set(this.formatTime(this.tourLog().timeOfTour))
    this.dateString.set(this.formatDate(this.tourLog().timeOfTour))
  }

  formatTime(date: Date): string {
    return date.toLocaleTimeString('de-AT', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    });
  }
  formatDate(date: Date): string {
    let returnDate:Date=new Date();
    returnDate.setFullYear(date.getFullYear(), date.getMonth()-1, date.getDate());
    return returnDate.toISOString().substring(0, 10);
  }

  submit(){
    const timeOfTourString=this.dateString()+"T"+ this.timeString()+":00";
    console.log(timeOfTourString);
    const timeOfTour=new Date(timeOfTourString);
    timeOfTour.setMonth(timeOfTour.getMonth()+1);
    const editLog=new TourLog(this.tourLog().id, this.tourName()==""?this.tourLog().name:this.tourName(), 
    timeOfTour, this.difficulty()??this.tourLog().difficulty, this.distance()??this.tourLog().totalDistanceInM,
     this.time()??this.tourLog().totalTimeInM, this.rating()??this.tourLog().rating, this.comment()==""?this.tourLog().comment: this.comment())

    this.tourService.editTourLog(this.tour().id, editLog);
    this.overlay.close();
  }
  parseFloatValue(value: string): number {
    return parseFloat(value);
  }
}
