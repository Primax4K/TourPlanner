import { Component, OnInit } from '@angular/core';
import { Tour } from '../../model/model';
import { TourListItem } from '../tour-list-item/tour-list-item';
import { TourService } from '../../services/TourService';
import { LoginService } from '../../services/LoginService';

@Component({
  selector: 'app-tour-list',
  imports: [TourListItem],
  templateUrl: './tour-list.html',
  styleUrl: './tour-list.css',
})
export class TourList {
  constructor(public tourService: TourService, public loginService:LoginService) {}
  async onDrop(event: DragEvent) {
    event.preventDefault();

    const file = event.dataTransfer?.files?.[0];
    if (!file) return;

    const text = await file.text();
    console.log(text)

    await this.tourService.importTour(text);
    alert("Imported");
}
}
