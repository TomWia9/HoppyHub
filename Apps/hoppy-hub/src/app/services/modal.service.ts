import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export enum ModalType {
  Login,
  Register,
  BeersFilters,
  BreweriesFilters,
  UpsertOpinion
}

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  modalOpened = new Subject<ModalType>();

  openModal(modalType: ModalType) {
    this.modalOpened.next(modalType);
  }
}
