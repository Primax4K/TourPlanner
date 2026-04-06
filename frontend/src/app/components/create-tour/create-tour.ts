import { Component, signal } from '@angular/core';
import { Tour } from '../../model/model';
import { TourService } from '../../services/TourService';
@Component({
  selector: 'app-create-tour',
  imports: [],
  templateUrl: './create-tour.html',
  styleUrl: './create-tour.css',
})
export class CreateTour {
  constructor(public tourService:TourService){}
  tourName = signal('');
  difficulty = signal<number | null>(null);
  transport = signal<string>('');
  description = signal('');
  start_long = signal<number|null>(null);
  start_lat = signal<number|null>(null);
  end_long = signal<number|null>(null);
  end_lat = signal<number|null>(null);
  submit() {
    this.tourService.createTour(-1, this.start_long()??0,  this.start_lat()??0, this.end_long()??0, this.end_lat()??0,this.tourName())
  }
  
  parseFloatValue(value: string): number {
    return parseFloat(value);
  }
}
