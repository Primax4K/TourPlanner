import { Injectable, signal } from '@angular/core';
import { Tour } from '../model/model';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  tours = signal<Tour[]>([
    { id: 1, name: 'Stadtführung', from_lat: 48.2082, from_long: 16.3738, to_lat: 47.2082, to_long:15.3738},
    { id: 2, name: 'Bergtour',from_lat: 48.2082, from_long: 16.3738, to_lat: 47.2082, to_long:15.3738},
  ]);

  selectedTour = signal<Tour | null>(null);
  selectTour(tourId: number) {
    this.selectedTour.set(this.tours().find(t => t.id === tourId) || null);
  }

}