import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ModalModel } from '../shared/modal-model';

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  modalOpened = new Subject<ModalModel>();

  openModal(modalModel: ModalModel): void {
    this.modalOpened.next(modalModel);
  }
}
