import { Injectable, signal } from '@angular/core';
import { RouteData, Tour } from '../model/model';
import * as polyline from '@mapbox/polyline';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  constructor(){
    const jwt_token="test"
    if(jwt_token){
      this.fetchAllTours(jwt_token)
    }
  }
  tours = signal<Tour[]>([]);
  tourLogView = signal<Tour|null>(null);
  selectedTour = signal<Tour | null>(null);
  selectTour(tourId: number) {
    this.selectedTour.set(this.tours().find(t => t.id === tourId) || null);
  }

  async createTour(id:number, from_lat:number, from_long:number, 
    to_lat:number, to_long:number, name:string){
      const routeInfo=await this.getRouteForTour(from_lat, from_long, to_lat, to_long)
      this.tours.update(tours=>[...tours,new Tour(id, name, from_long, from_lat, to_long, to_lat, routeInfo)])
      
  }
  async editTour(editedTour:Tour){
    const routeInfo=await this.getRouteForTour(
      editedTour.from_lat, editedTour.from_long, editedTour.to_lat, editedTour.to_long);
      editedTour.routeInfo=routeInfo;
    this.tours.update(tours =>
      tours.map(t =>
        t.id === editedTour.id ? editedTour : t
      ));
      this.selectTour(editedTour.id)
  }
  async fetchAllTours(jwt_token:string){
    this.createTour(1,48.2082,16.3738,48.2082,16.358,"Wien Tour 1");
    this.createTour(2,48.2082,16.3738,48.2082,16.3938,"Wien Tour 2");
  }
  async deleteTour(tourId:number){
    this.tours.update(tours=>
      tours.filter(t => t.id !== tourId)
    );
  }
  private async getRouteForTour(
    from_long: number, from_lat: number, 
    to_long: number, to_lat: number
  ): Promise<RouteData> {
    
    const api_key = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6IjM2NTRjM2U5YTEwOTQyMGZhM2VhNGVkYjZlNDg2ZmMwIiwiaCI6Im11cm11cjY0In0=";
       
    const url = 'https://api.openrouteservice.org/v2/directions/driving-car'

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Authorization': api_key,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        coordinates: [
          [from_lat, from_long],
          [to_lat, to_long]
        ]
      })
    })
    
    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.error?.message || 'Routing Fehler');
    }

    const data = await response.json();
    const route = data.routes?.[0];

    if (!route) {
      throw new Error('Keine Route gefunden');
    }
    
    return {
      distance: route.summary.distance,
      duration: route.summary.duration,
      coordinates: polyline.decode(route.geometry)
    };
  }
}
