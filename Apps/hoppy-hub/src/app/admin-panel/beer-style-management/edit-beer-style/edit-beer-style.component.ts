import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BeerStylesService } from '../../../beer-styles/beer-styles.service';
import { BeerStyle } from '../../../beer-styles/beer-style.model';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ModalType } from '../../../shared/model-type';
import { BeerStylesParams } from '../../../beer-styles/beer-styles-params';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, tap, map, switchMap } from 'rxjs';
import { UpsertBeerStyleCommand } from '../../../beer-styles/upsert-beer-style-command.model';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { DeleteBeerStyleModalComponent } from '../delete-beer-style-modal/delete-beer-style-modal.component';

@Component({
  selector: 'app-edit-beer-style',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    DeleteBeerStyleModalComponent
  ],
  templateUrl: './edit-beer-style.component.html'
})
export class EditBeerStyleComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private router: Router = inject(Router);
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private alertService: AlertService = inject(AlertService);
  private modalService: ModalService = inject(ModalService);
  private destroy$ = new Subject<void>();

  beerStyle!: BeerStyle;
  error = '';
  loading = true;
  beerStyleForm!: FormGroup;

  ngOnInit(): void {
    this.getBeerStyle();
  }

  onBeerStyleDelete(): void {
    this.modalService.openModal(
      new ModalModel(ModalType.DeleteBeerStyle, {
        beerStyleId: this.beerStyle.id
      })
    );
  }

  onFormSave(): void {
    this.loading = true;

    if (!this.beerStyleForm.pristine) {
      const upsertBeerStyleCommand = this.beerStyleForm
        .value as UpsertBeerStyleCommand;
      upsertBeerStyleCommand.id = this.beerStyle.id;

      this.beerStylesService
        .updateBeerStyle(this.beerStyle.id, upsertBeerStyleCommand)
        .pipe(
          takeUntil(this.destroy$),
          tap({
            next: () => {
              this.alertService.openAlert(
                AlertType.Success,
                'Beer style updated successfully'
              );
              this.getBeerStyle();
              this.beerStyleChanged();
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

  beerStyleDeleted(): void {
    this.beerStyleChanged();
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private getBeerStyle(): void {
    this.loading = true;
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        switchMap(beerStyleId =>
          this.beerStylesService.getBeerStyleById(beerStyleId as string).pipe(
            tap({
              next: (beerStyle: BeerStyle) => {
                this.beerStyle = beerStyle;
                this.initForm(beerStyle);
                this.error = '';
                window.scrollTo({ top: 0, behavior: 'smooth' });
                this.loading = false;
              },
              error: () => {
                this.error = 'An error occurred while loading the beer style';
                this.loading = false;
              }
            })
          )
        )
      )
      .subscribe();
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

  private initForm(beerStyle: BeerStyle): void {
    this.beerStyleForm = new FormGroup({
      name: new FormControl(beerStyle.name, [
        Validators.required,
        Validators.maxLength(100)
      ]),
      description: new FormControl(beerStyle.description, [
        Validators.required,
        Validators.maxLength(1000)
      ]),
      countryOfOrigin: new FormControl(beerStyle.countryOfOrigin, [
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
