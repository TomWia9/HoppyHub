import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BeerStylesService } from '../../../beer-styles/beer-styles.service';
import { UpsertBeerStyleCommand } from '../../../beer-styles/upsert-beer-style-command.model';
import { BeerStylesParams } from '../../../beer-styles/beer-styles-params';
import { HttpErrorResponse } from '@angular/common/http';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { Subject, takeUntil, tap } from 'rxjs';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';

@Component({
  selector: 'app-new-beer-style',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule
  ],
  templateUrl: './new-beer-style.component.html'
})
export class NewBeerStyleComponent implements OnInit, OnDestroy {
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private alertService: AlertService = inject(AlertService);
  private destroy$ = new Subject<void>();

  error = '';
  loading = false;
  newBeerStyleForm!: FormGroup;

  ngOnInit(): void {
    this.initForm();
  }

  onFormSave(): void {
    this.loading = true;
    const upsertBeerStyleCommand = this.newBeerStyleForm
      .value as UpsertBeerStyleCommand;

    if (!this.newBeerStyleForm.pristine) {
      this.beerStylesService
        .createBeerStyle(upsertBeerStyleCommand)
        .pipe(
          takeUntil(this.destroy$),
          tap({
            next: () => {
              this.alertService.openAlert(
                AlertType.Success,
                'Beer style created successfully'
              );
              this.beerStyleChanged();
              this.newBeerStyleForm.reset();
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

  beerStyleChanged(): void {
    this.beerStylesService.paramsChanged.next(
      new BeerStylesParams({
        pageSize: 10,
        pageNumber: 1
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
    this.newBeerStyleForm = new FormGroup({
      name: new FormControl('', [
        Validators.required,
        Validators.maxLength(100)
      ]),
      description: new FormControl('', [
        Validators.required,
        Validators.maxLength(1000)
      ]),
      countryOfOrigin: new FormControl('', [
        Validators.required,
        Validators.maxLength(500)
      ])
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
