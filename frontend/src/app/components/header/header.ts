import { Component } from '@angular/core';
import { OverlayService } from '../../services/OverlayService';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  constructor(public overlay: OverlayService){}
  login(){
    this.overlay.open('login')
  }
  register(){
    this.overlay.open('register')
  }
}
