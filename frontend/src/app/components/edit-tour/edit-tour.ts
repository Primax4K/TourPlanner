import { Component, input, signal } from '@angular/core';
import { Tour } from '../../model/model';
import { TourService } from '../../services/TourService';
import { OverlayService } from '../../services/OverlayService';

@Component({
  selector: 'app-edit-tour',
  imports: [],
  templateUrl: './edit-tour.html',
  styleUrl: './edit-tour.css',
})
export class EditTour {
  constructor(public tourService:TourService, public overlay:OverlayService){}
  tour=input.required<Tour>()
  
  tourName = signal('');
  difficulty = signal<number | null>(null);
  transport = signal<string>('');
  description = signal('');
  start_long = signal<number|null>(null);
  start_lat = signal<number|null>(null);
  end_long = signal<number|null>(null);
  end_lat = signal<number|null>(null);

  parseFloatValue(value: string): number {
    return parseFloat(value);
  }
  submit() {
  this.tourService.editTour(new Tour(this.tour().id, this.tourName()==""?this.tour().name:this.tourName(),
  this.start_long()??this.tour().from_long, this.start_lat()??this.tour().from_lat,
  this.end_long()??this.tour().to_long, this.end_lat()??this.tour().to_lat, null, this.tour().tourLogs, this.tour().description))  
  this.overlay.close();
  }
}
