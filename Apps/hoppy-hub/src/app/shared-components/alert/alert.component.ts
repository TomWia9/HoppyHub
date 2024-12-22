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
  faCircleExclamation
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
  icon = faCircleCheck;

  ngOnInit(): void {
    this.alertOpenedSubscription = this.alertService.alertOpened.subscribe(
      (alert: Alert) => {
        this.openAlert(alert);
      }
    );
  }

  openAlert(alert: Alert): void {
    switch (alert.alertType) {
      case AlertType.Success:
        this.alertClass = 'alert-success';
        this.icon = faCircleCheck;
        break;
      case AlertType.Error:
        this.alertClass = 'alert-error';
        this.icon = faCircleExclamation;
        break;
      case AlertType.Warning:
        this.alertClass = 'alert-warning';
        this.icon = faExclamation;
        break;
      case AlertType.Info:
        this.alertClass = 'alert-info';
        this.icon = faCircleInfo;
        break;
      default:
        this.alertClass = '';
        this.icon = faCircleInfo;
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

  onAlertClose(): void {
    this.message = '';
    this.alertClass = '';
    this.showAlert = false;
  }

  ngOnDestroy(): void {
    if (this.alertOpenedSubscription) {
      this.alertOpenedSubscription.unsubscribe();
    }
  }
}
