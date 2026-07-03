import { Component, input, signal } from '@angular/core';
import { Tour, TransportType } from '../../model/model';
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
    console.log(this.description())
    let transportType=TransportType.Car;
    switch(this.transport()){
      case 'car':
        transportType=TransportType.Car;
        break;
      case 'bicycle':
        transportType=TransportType.Cycling;
        break;
      case 'walking':
        transportType=TransportType.Walking;
        break;
      default:
        transportType=TransportType.Car
    }
    this.tourService.editTour(new Tour(this.tour().id, this.tourName()==""?this.tour().name:this.tourName(),
    this.start_long()??this.tour().from_long, this.start_lat()??this.tour().from_lat,
    this.end_long()??this.tour().to_long, this.end_lat()??this.tour().to_lat, null, this.tour().tourLogs, this.description(), transportType))
    this.overlay.close();
  }
}
