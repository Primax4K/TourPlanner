import { Component, input, signal } from '@angular/core';
import { TourList } from './components/tour-list/tour-list';
import { TourMap } from './components/tour-map/tour-map';
import { TourToolbar } from './components/tour-toolbar/tour-toolbar';
import { Header } from './components/header/header';
import { LoginComp } from './components/login/login';
import { OverlayService } from './services/OverlayService';
import { Register } from './components/register/register';
import { CreateTour } from './components/create-tour/create-tour';
import { EditTour } from './components/edit-tour/edit-tour';
import { TourService } from './services/TourService';
import { Tour } from './model/model';
import { TourLogList } from './components/tour-log-list/tour-log-list';
import { EditLog } from './components/edit-log/edit-log';
import { CreateLog } from './components/create-log/create-log';

@Component({
  selector: 'app-root',
  imports: [TourList, Header, TourMap, TourToolbar, LoginComp, Register, CreateTour, EditTour, TourLogList, EditLog, CreateLog],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(public overlay: OverlayService, public tourService:TourService) {}
}
