import { AlertType } from './alert.service';

export interface Alert {
  alertType: AlertType;
  message: string;
}
