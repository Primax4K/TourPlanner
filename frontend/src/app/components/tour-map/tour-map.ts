import { AfterViewInit, Component, effect, OnDestroy } from '@angular/core';
import { TourService } from '../../services/TourService';

import * as L from 'leaflet';
import { Tour } from '../../model/model';

@Component({
  selector: 'app-tour-map',
  imports: [],
  templateUrl: './tour-map.html',
  styleUrl: './tour-map.css',
})
export class TourMap implements AfterViewInit{

  private map!: L.Map;

  constructor(public tourService: TourService) {}

  ngAfterViewInit(): void {
    this.initMap();
  }

  private initMap(): void {
    this.map = L.map('map').setView([48.2082, 16.3738], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);
  }
}
