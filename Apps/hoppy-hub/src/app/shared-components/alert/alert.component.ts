import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { AlertService, AlertType } from './alert.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Alert } from './alert.model';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faCircleCheck,
  faCircleInfo,
  faExclamation,
  faXmark
} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule, FontAwesomeModule],
  templateUrl: './alert.component.html'
})
export class AlertComponent implements OnInit, OnDestroy {
  private alertService: AlertService = inject(AlertService);

  message: string = '';
  alertClass: string = '';
  alertType: string = '';
  showAlert: boolean = false;
  alertOpenedSubscription!: Subscription;
  faCircleCheck = faCircleCheck;
  faXmark = faXmark;
  faExclamation = faExclamation;
  faCircleInfo = faCircleInfo;

  ngOnInit(): void {
    this.alertOpenedSubscription = this.alertService.alertOpened.subscribe(
      (alert: Alert) => {
        this.openAlert(alert);
      }
    );
  }
  openAlert(alert: Alert) {
    switch (alert.alertType) {
      case AlertType.Success:
        this.alertClass = 'alert-success';
        this.alertType = 'success';
        break;
      case AlertType.Error:
        this.alertClass = 'alert-error';
        this.alertType = 'error';
        break;
      case AlertType.Warning:
        this.alertClass = 'alert-warning';
        this.alertType = 'warning';
        break;
      case AlertType.Info:
        this.alertClass = 'alert-info';
        this.alertType = 'info';
        break;
      default:
        this.alertClass = '';
        this.alertType = '';
        break;
    }

    this.message = alert.message;
    this.showAlert = true;
    if (alert.alertType != AlertType.Error) {
      setTimeout(() => {
        this.onAlertClose();
      }, 3500);
    }
  }

  onAlertClose() {
    this.message = '';
    this.alertClass = '';
    this.showAlert = false;
  }

  ngOnDestroy(): void {
    this.alertOpenedSubscription.unsubscribe();
  }
}
