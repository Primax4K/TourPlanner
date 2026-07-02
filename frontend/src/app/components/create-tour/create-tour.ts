import { Component, signal } from '@angular/core';
import { Tour, TransportType } from '../../model/model';
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
  transport = signal<string>('');
  description = signal('');
  start_long = signal<number|null>(null);
  start_lat = signal<number|null>(null);
  end_long = signal<number|null>(null);
  end_lat = signal<number|null>(null);
  submit() {
    let transportType=TransportType.Car;
        switch(this.transport()){
          case 'car':
            transportType=TransportType.Car;
            break;
          case 'bicycle':
            transportType=TransportType.Cycling;
            break;
            break;
          case 'walking':
            transportType=TransportType.Walking;
            break;
          default:
            transportType=TransportType.Car
        }
        
    if(this.tourName()!==""&&this.start_long()&&this.start_lat()&&this.end_long()&&this.end_lat()){
      this.tourService.createTour(new Tour("",this.tourName(), this.start_long()??0,  this.start_lat()??0, this.end_long()??0, this.end_lat()??0,null,[],this.description(),transportType));
    }
    else{
      alert("invalid fields");
    }
  }
  
  parseFloatValue(value: string): number {
    return parseFloat(value);
  }
}
