import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginModalService {
  modalOpened = new Subject<void>();

  openModal() {
    this.modalOpened.next();
  }
}
