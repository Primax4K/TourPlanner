import { Component, effect, signal } from '@angular/core';
import { OverlayService } from '../../services/OverlayService';
import { LoginService } from '../../services/LoginService';
import { TourService } from '../../services/TourService';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  constructor(public overlay: OverlayService, public loginService:LoginService, public tourService:TourService){
  }
  
  searchQuery = signal('');
  
  login(){
    this.overlay.open('login');
  }
  register(){
    this.overlay.open('register');
  }
  logout(){
    this.loginService.clearToken();
    this.tourService.selectTour("");
  }
  search(){
    if(this.searchQuery()==""){
      this.tourService.fetchAllTours();
    }
    else{
      this.tourService.searchTour(this.searchQuery());
    }
  }
  searchLog(){
    if(this.searchQuery()==""){
      this.tourService.fetchAllTours();
      const selectedTour=this.tourService.selectedTour();
      if(selectedTour!=null){
        this.tourService.selectTour(selectedTour.id)
      }
    }
    else{
      const selectedTour=this.tourService.selectedTour();
      if(selectedTour!=null){
        this.tourService.searchTourLog(selectedTour.id,this.searchQuery());
      }
    }
  }
}
