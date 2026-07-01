import { Component, signal } from '@angular/core';
import { LoginService } from '../../services/LoginService';
import { OverlayService } from '../../services/OverlayService';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComp {
  constructor(public loginService:LoginService, public overlayService:OverlayService){}

  usernameOrEmail = signal('');
  password = signal('');

  async submit(){
    await this.loginService.Login(this.usernameOrEmail(), this.password())
    if(this.loginService.getToken()!=null){
      this.overlayService.close()
    }
  }
}
