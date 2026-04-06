import { AfterViewInit, Component, effect } from '@angular/core';
import { TourService } from '../../services/TourService';

import * as L from 'leaflet';
import { RouteData, Tour } from '../../model/model';

@Component({
  selector: 'app-tour-map',
  imports: [],
  templateUrl: './tour-map.html',
  styleUrl: './tour-map.css',
})
export class TourMap implements AfterViewInit{

  private map!: L.Map;
  private from!:L.Marker;
  private to!:L.Marker;
  private route!:L.Polyline;

  private start = L.icon({
    iconUrl: 'start.png',
    iconSize: [25, 25],
    iconAnchor: [12, 41],
  });
  private finish = L.icon({
    iconUrl: 'finish.png',
    iconSize: [25, 25],
    iconAnchor: [12, 41],
  });



  constructor(public tourService: TourService) {
    effect(()=>{
      const selected=tourService.selectedTour()
      if(selected){
        this.mapPosition(selected.from_lat, selected.from_long)
        this.setFrom(selected.from_lat, selected.from_long)
        this.setTo(selected.to_lat, selected.to_long)
        this.drawRoute(selected.routeInfo)
      }
    })
  }

  ngAfterViewInit(): void {
    this.initMap();
  }

  private initMap(): void {
    this.map = L.map('map').setView([48.2082, 16.3738], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);
  }
  private mapPosition(lat:number, long:number): void{
    this.map.flyTo([lat, long], this.map.getZoom(), {
      animate: true,
      duration: 1.5
    });
  }
  private drawRoute(routeInfo:RouteData|null){
    if (this.route) {
      this.map.removeLayer(this.route);
    }
    if(routeInfo){
    this.route = L.polyline(routeInfo.coordinates, {
      color: 'red',
      weight: 5
    }).addTo(this.map);
    }
  }
  private setFrom(lat:number, long:number){
    if(this.from){
      this.from.setLatLng([lat,long])
      return
    }
    const icon=this.start
    this.from = L.marker([lat,long], {icon}).addTo(this.map);
  }
  private setTo(lat:number, long:number){
    if(this.to){
      this.to.setLatLng([lat,long])
      return
    }
    const icon=this.finish
    this.to = L.marker([lat,long],{ icon }).addTo(this.map);
  }
}
