import { effect, Injectable, signal } from '@angular/core';
import { RouteData, createTourDto, Tour, TourLog, TransportType, receiveTourDto, editTourDto, createTourLogDto, receiveTourLogDto } from '../model/model';
import * as polyline from '@mapbox/polyline';
import { LoginService } from './LoginService';
import { environment } from '../environment';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  constructor(public logService: LoginService){
    effect(() => {
      if (this.logService.isLoggedIn()) {
        this.fetchAllTours();
      } else {
        this.tours.set([]);
      }
    });
    this.logService=logService; 
    this.fetchAllTours()
  }
  tours = signal<Tour[]>([]);
  tourLogView = signal<boolean>(false)
  selectedTourLog = signal<TourLog | null>(null);
  selectedTour = signal<Tour | null>(null);
  newIdCounter=0;

  sortTourByPopularity(tours:Tour[]):Tour[]{
    return tours.sort((a,b)=>b.getPopularity()-a.getPopularity())
  }
  selectTour(tourId: string) {
    this.selectedTour.set(this.tours().find(t => t.id === tourId) || null);
  }
  selectTourLog(tourLogId: string) {
    if(this.selectedTour()){
      this.selectedTourLog.set(this.selectedTour()!.tourLogs.find(tl=>tl.id==tourLogId)??null)
    }
  }
  
  async fetchAllTours(){
    const token=this.logService.getToken()
    if(token==null) return;

    
    const response = await fetch(`${environment.apiUrl}/api/tour/mine`, {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });

    if(!response.ok) return;

    const data = await response.json();
    const fetchedTours: Tour[] = data.map(receiveTourDto);
    this.tours.set(fetchedTours)
  }
  async searchTour(query:string){
    const token=this.logService.getToken()
    if(token==null) return;

    
    const response = await fetch(`${environment.apiUrl}/api/tour/search?q=${""+encodeURIComponent(query)}`, {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });

    if(!response.ok) return;

    const data = await response.json();
    const fetchedTours: Tour[] = data.map(receiveTourDto);
    this.tours.set(fetchedTours)
  }
  async createTour(newTour:Tour){
    const token=this.logService.getToken()
    if(token==null) return;

    const response = await fetch(`${environment.apiUrl}/api/tour/`, {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify(createTourDto(newTour))
    });

    if(!response.ok) return;

    const data = await response.json();
    const fetchedTour: Tour = receiveTourDto(data);

    this.tours.update(tours=>[...tours,fetchedTour]);
  }

  async importTour(json:string){
    const token=this.logService.getToken()
    if(token==null) return;

    const response = await fetch(`${environment.apiUrl}/api/tour/`, {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: json
    });

    if(!response.ok) return;

    const data = await response.json();
    const fetchedTour: Tour = receiveTourDto(data);

    this.tours.update(tours=>[...tours,fetchedTour]);
  }

  async createRandomTour(){
    const cities = [
      { name: 'Wien',        long: 16.3738, lat: 48.2082 },
      { name: 'Graz',        long: 15.4395, lat: 47.0707 },
      { name: 'Linz',        long: 14.2858, lat: 48.3069 },
      { name: 'Salzburg',    long: 13.0550, lat: 47.8095 },
      { name: 'Innsbruck',   long: 11.4041, lat: 47.2692 },
      { name: 'Klagenfurt',  long: 14.3122, lat: 46.6247 },
      { name: 'Villach',     long: 13.8558, lat: 46.6111 },
      { name: 'Wels',        long: 14.0231, lat: 48.1575 },
      { name: 'Sankt Pölten',long: 15.6250, lat: 48.2047 },
      { name: 'Dornbirn',    long:  9.7439, lat: 47.4125 },
      { name: 'Bregenz',     long:  9.7471, lat: 47.5031 },
      { name: 'Steyr',       long: 14.4213, lat: 48.0407 },
      { name: 'Feldkirch',   long:  9.6000, lat: 47.2333 },
      { name: 'Eisenstadt',  long: 16.5188, lat: 47.8457 },
    ];

    const fromIdx = Math.floor(Math.random() * cities.length);
    let toIdx = Math.floor(Math.random() * cities.length);
    while (toIdx === fromIdx) {
      toIdx = Math.floor(Math.random() * cities.length);
    }

    const from = cities[fromIdx];
    const to = cities[toIdx];

    await this.createTour(new Tour(
      "",
      `${from.name} → ${to.name}`,
      from.long, from.lat,
      to.long, to.lat,
      null,
      [],
      `Random car tour from ${from.name} to ${to.name}`,
      TransportType.Car
    ));
  }

  async exportTour(tour:Tour){
    const data = createTourDto(tour);

    const json = JSON.stringify(data, null, 2);

    const blob = new Blob([json], { type: "application/json" });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = `tour-${tour.id ?? "export"}.json`;

    document.body.appendChild(a);
    a.click();
    a.remove();

    URL.revokeObjectURL(url);
  }
  async deleteTour(tourId:string){
    const token=this.logService.getToken()
    if(token==null) return;

    const response = await fetch(`${environment.apiUrl}/api/tour/${tourId}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      }
    });

    if(!response.ok) return;
  
    this.tours.update(tours=>
        tours.filter(t => t.id !== tourId)
      );
  }

  async editTour(editedTour:Tour){
    const token=this.logService.getToken()
    if(token==null) return;
    console.log(editedTour.id)
    const response = await fetch(`${environment.apiUrl}/api/tour/${editedTour.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },      
      body: JSON.stringify(editTourDto(editedTour))
    });

    if(!response.ok) return

    const data = await response.json();
    const responseTour: Tour = receiveTourDto(data);
    this.tours.update(tours =>
      tours.map(t =>
        t.id === responseTour.id ? responseTour : t
      ));
    this.selectTour(responseTour.id)
  }

  async createTourLog(tourId:string, tourLog:TourLog){

    const token=this.logService.getToken()
    if(token==null) return;

    const response = await fetch(`${environment.apiUrl}/api/tourlog`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          },      
          body: JSON.stringify(createTourLogDto(tourId, tourLog))
        });

    if(!response.ok) return

    const data = await response.json();

    this.tours.update(tours=>tours.map(tour=>{
      if(tour.id!==tourId){
        return tour;
      };
      return new Tour(
          tour.id,
          tour.name,
          tour.from_long,
          tour.from_lat,
          tour.to_long,
          tour.to_lat,
          tour.routeInfo,
          [...tour.tourLogs, receiveTourLogDto(data)],
          tour.description,
          tour.transport
        );
      }));
      this.selectTour(tourId);
  }
  async searchTourLog(tourId:string, query:string){
    const token=this.logService.getToken()
    if(token==null) return;

    
    const response = await fetch(`${environment.apiUrl}/api/tourlog/search?q=${encodeURIComponent(query)}`, {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });

    if(!response.ok) return;

    const data = await response.json();
    this.tours.update(tours=>
      tours.map(t =>
        t.id !== tourId ? t :
        new Tour(t.id, t.name, t.from_long, t.from_lat, t.to_long, t.to_lat, t.routeInfo,
          data.map(receiveTourLogDto), t.description, t.transport)
    ));
    this.selectTour(tourId);
  }
  
  async editTourLog(tourId:string, editedTourLog:TourLog){
    const token=this.logService.getToken()
    if(token==null) return;

    console.log(tourId);
    const response = await fetch(`${environment.apiUrl}/api/tourlog/${editedTourLog.id}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          },      
          body: JSON.stringify(createTourLogDto(tourId, editedTourLog))
        });

    if(!response.ok) return

    const data = await response.json();
    this.tours.update(tours=>
      tours.map(t =>
        t.id !== tourId ? t : 
        new Tour(t.id, t.name, t.from_long, t.from_lat, t.to_long, t.to_lat, t.routeInfo,
          t.tourLogs.map(tl=>
            tl.id === data.id ? editedTourLog : tl), t.description, t.transport)
      )
    );

    this.selectTour(tourId);
  }
  

  async deleteTourLog(tourId:string, tourLogId:string){
    const token=this.logService.getToken()
    if(token==null) return;

    console.log(tourId);
    const response = await fetch(`${environment.apiUrl}/api/tourlog/${tourLogId}`, {
          method: "DELETE",
          headers: {
            "Authorization": `Bearer ${token}`
          }
        });

    if(!response.ok) return
    this.tours.update(tours=>
      tours.map(t =>
        t.id !== tourId ? t : 
        new Tour(t.id, t.name, t.from_long, t.from_lat, t.to_long, t.to_lat, t.routeInfo, t.tourLogs.filter(tl=>tl.id!==tourLogId), t.description, t.transport)
      )
    );
    this.selectTour(tourId);
  }
}
