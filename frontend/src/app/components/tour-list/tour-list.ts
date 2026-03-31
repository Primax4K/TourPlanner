import { Component, OnInit } from '@angular/core';
import { Tour } from '../../model/model';
import { TourListItem } from '../tour-list-item/tour-list-item';
import { TourService } from '../../services/TourService';

@Component({
  selector: 'app-tour-list',
  imports: [TourListItem],
  templateUrl: './tour-list.html',
  styleUrl: './tour-list.css',
})
export class TourList {
  constructor(public tourService: TourService) {}
}
