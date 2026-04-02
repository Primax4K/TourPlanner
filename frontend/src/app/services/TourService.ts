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
      this.loadAllUsersTours(jwt_token)
    }
  }
  tours = signal<Tour[]>([]);

  selectedTour = signal<Tour | null>(null);
  selectTour(tourId: number) {
    this.selectedTour.set(this.tours().find(t => t.id === tourId) || null);
  }

  async createTour(from_lat:number, from_long:number, 
    to_lat:number, to_long:number, name:string){
      const routeInfo=await this.getRouteForTour(from_lat, from_long, to_lat, to_long)
      const tour = new Tour(-1, name, from_long, from_lat, to_long, to_lat, routeInfo)
  }
  async fetchAllTours(jwt_token:string){
    const fetchedTours:Tour[]=[
      { id: 1, name: 'Stadtführung', from_lat: 48.2082, from_long: 16.3738, to_lat: 48.2082, to_long:16.358, routeInfo: null},
      { id: 2, name: 'Bergtour',from_lat: 48.2082, from_long: 16.3738, to_lat: 48.2082, to_long:16.3938, routeInfo: null},
    ]
    fetchedTours.forEach(async (tour,idx)=>{
      if(tour.routeInfo==null){
        fetchedTours[idx].routeInfo = await this.getRouteForTour(tour.from_lat, tour.from_long, tour.to_lat, tour.to_long)
      }
    })
    return fetchedTours
  }
  async loadAllUsersTours(jwt_token:string){
    this.tours.set(await this.fetchAllTours(jwt_token))
  }
  private async getRouteForTour(
    from_lat: number, from_long: number, 
    to_lat: number, to_long: number
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
          [from_long, from_lat],
          [to_long, to_lat]
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
