import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { AlertService, AlertType } from './alert.service';
import { Subscription } from 'rxjs';
import { Alert } from './Alert';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.component.html'
})
export class AlertComponent implements OnInit, OnDestroy {
  private alertService: AlertService = inject(AlertService);
  message: string = '';
  alertClass: string = '';
  showAlert: boolean = false;
  alertOpenedSubscription!: Subscription;

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
        break;
      case AlertType.Error:
        this.alertClass = 'alert-error';
        break;
      case AlertType.Warning:
        this.alertClass = 'alert-warning';
        break;
      case AlertType.Info:
        this.alertClass = 'alert-info';
        break;
      default:
        this.alertClass = '';
        break;
    }

    this.message = alert.message;
    this.showAlert = true;
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
