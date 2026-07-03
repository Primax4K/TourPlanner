import { Injectable, signal } from '@angular/core';

type OverlayType = 'edit' | 'create' | 'delete' | 'login' | 'register' | 'deleteLog' | 'createLog'| 'editLog' | null;

@Injectable({ providedIn: 'root' })
export class OverlayService {

  type = signal<OverlayType>(null);
  
  open(type: OverlayType) {
    this.type.set(type);
  }

  close() {
    this.type.set(null);
  }
}