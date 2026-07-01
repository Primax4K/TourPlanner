import { Component, signal } from '@angular/core';
import { LoginService } from '../../services/LoginService';
import { OverlayService } from '../../services/OverlayService';

@Component({
  selector: 'app-register',
  imports: [],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  constructor(public loginService:LoginService, public overlayService:OverlayService){}

  username = signal('');
  email = signal('');
  password = signal('');

  async submit(){
    await this.loginService.Register(this.username(), this.email(), this.password());
    if(this.loginService.getToken()!=null){
      this.overlayService.close()
    }
  }
}
