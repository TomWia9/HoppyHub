import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { Alert } from './Alert';

export enum AlertType {
  Success,
  Warning,
  Error,
  Info
}

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  alertOpened = new Subject<Alert>();

  openAlert(alertType: AlertType, message: string) {
    const alert: Alert = {
      alertType: alertType,
      message: message
    };

    this.alertOpened.next(alert);
  }
}
