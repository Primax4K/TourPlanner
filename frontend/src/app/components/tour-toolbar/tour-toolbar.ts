import { Component } from '@angular/core';
import { OverlayService } from '../../services/OverlayService';
import { TourService } from '../../services/TourService';
import { LoginService } from '../../services/LoginService';

@Component({
  selector: 'app-tour-toolbar',
  imports: [],
  templateUrl: './tour-toolbar.html',
  styleUrl: './tour-toolbar.css',
})
export class TourToolbar {
  constructor(public overlay: OverlayService, public tourService: TourService, public loginService:LoginService){}
    create_tour(){
      this.overlay.open('create')
    }
    edit_tour(){
      this.overlay.open('edit')
    }
    delete_tour(){
      this.overlay.open('delete')
    }
    create_tourLog(){
      this.overlay.open('createLog');
    }
    edit_tourLog(){
      this.overlay.open('editLog');
    }
    delete_tourLog(){
      this.overlay.open('deleteLog');
    }
}
