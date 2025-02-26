import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { HttpErrorResponse } from '@angular/common/http';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { Subject, takeUntil, tap } from 'rxjs';

import { BreweriesParams } from '../../../breweries/breweries-params';
import { BreweriesService } from '../../../breweries/breweries.service';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { CommonModule } from '@angular/common';
import { UpsertBreweryCommand } from '../../../breweries/upsert-brewery-command.model';

@Component({
  selector: 'app-new-brewery',
  standalone: true,
  imports: [
    FontAwesomeModule,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './new-brewery.component.html'
})
export class NewBreweryComponent implements OnInit, OnDestroy {
  private breweriesService: BreweriesService = inject(BreweriesService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  error = '';
  loading = false;
  newBreweryForm!: FormGroup;

  ngOnInit(): void {
    this.initForm();
  }

  onFormSave(): void {
    this.loading = true;
    const upsertBreweryCommand = this.newBreweryForm
      .value as UpsertBreweryCommand;

    if (!this.newBreweryForm.pristine) {
      this.breweriesService
        .createBrewery(upsertBreweryCommand)
        .pipe(
          takeUntil(this.destroy$),
          tap({
            next: () => {
              this.alertService.openAlert(
                AlertType.Success,
                'Brewery created successfully'
              );
              this.breweryChanged();
              this.newBreweryForm.reset();
            },
            error: error => {
              this.handleError(error);
            },
            complete: () => {
              this.loading = false;
            }
          })
        )
        .subscribe();
    }
  }

  breweryChanged(): void {
    this.breweriesService.paramsChanged.next(
      new BreweriesParams({
        pageSize: 10,
        pageNumber: 1,
        sortBy: 'foundationYear',
        sortDirection: 1
      })
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = null;

    if (error.error) {
      const firstKey = Object.keys(error.error?.errors)[0] ?? null;
      const firstValueArray = error.error?.errors[firstKey] as string[];
      errorMessage = firstValueArray[0];
    }

    if (!errorMessage) {
      this.alertService.openAlert(AlertType.Error, 'Something went wrong');
    } else {
      this.alertService.openAlert(AlertType.Error, errorMessage);
    }

    this.loading = false;
  }

  private initForm(): void {
    this.newBreweryForm = new FormGroup({
      name: new FormControl('', [
        Validators.required,
        Validators.maxLength(500)
      ]),
      description: new FormControl('', Validators.maxLength(5000)),
      foundationYear: new FormControl(
        new Date().getFullYear(),
        Validators.required
      ),
      websiteUrl: new FormControl('', Validators.maxLength(200)),
      street: new FormControl('', [
        Validators.required,
        Validators.minLength(200)
      ]),
      number: new FormControl('', [
        Validators.required,
        Validators.minLength(10)
      ]),
      postcode: new FormControl('', [
        Validators.required,
        Validators.minLength(3)
      ]),
      city: new FormControl('', [
        Validators.required,
        Validators.minLength(50)
      ]),
      state: new FormControl('', [
        Validators.required,
        Validators.minLength(50)
      ]),
      country: new FormControl('', [
        Validators.required,
        Validators.minLength(50)
      ])
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
