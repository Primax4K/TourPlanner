import { Component, signal } from '@angular/core';
import { TourList } from './components/tour-list/tour-list';
import { TourMap } from './components/tour-map/tour-map';
import { TourToolbar } from './components/tour-toolbar/tour-toolbar';
import { Header } from './components/header/header';

@Component({
  selector: 'app-root',
  imports: [TourList, Header, TourMap, TourToolbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('frontend');
}
