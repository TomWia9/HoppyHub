import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BreweriesService } from '../../../breweries/breweries.service';
import { Brewery } from '../../../breweries/brewery.model';
import { UpsertBreweryCommand } from '../../../breweries/upsert-brewery-command.model';
import { BreweriesParams } from '../../../breweries/breweries-params';
import { HttpErrorResponse } from '@angular/common/http';
import {
  FormGroup,
  FormControl,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, map, switchMap, tap } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { DeleteBreweryModalComponent } from '../delete-brewery-modal/delete-brewery-modal.component';

@Component({
  selector: 'app-edit-brewery',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    ReactiveFormsModule,
    DeleteBreweryModalComponent
  ],
  templateUrl: './edit-brewery.component.html'
})
export class EditBreweryComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private router: Router = inject(Router);
  private breweriesService: BreweriesService = inject(BreweriesService);
  private alertService: AlertService = inject(AlertService);
  private modalService: ModalService = inject(ModalService);
  private destroy$ = new Subject<void>();

  brewery!: Brewery;
  error = '';
  loading = true;
  breweryForm!: FormGroup;

  ngOnInit(): void {
    this.getBrewery();
  }

  onBreweryDelete(): void {
    this.modalService.openModal(
      new ModalModel(ModalType.DeleteBrewery, {
        breweryId: this.brewery.id
      })
    );
  }

  onFormSave(): void {
    this.loading = true;

    if (!this.breweryForm.pristine) {
      const upsertBreweryCommand = this.breweryForm
        .value as UpsertBreweryCommand;
      upsertBreweryCommand.id = this.brewery.id;

      this.breweriesService
        .updateBrewery(this.brewery.id, upsertBreweryCommand)
        .pipe(
          takeUntil(this.destroy$),
          tap({
            next: () => {
              this.alertService.openAlert(
                AlertType.Success,
                'Brewery updated successfully'
              );
              this.getBrewery();
              this.breweryChanged();
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

  breweryDeleted(): void {
    this.breweryChanged();
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private getBrewery(): void {
    this.loading = true;
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        switchMap(beerId =>
          this.breweriesService.getBreweryById(beerId as string).pipe(
            tap({
              next: (brewery: Brewery) => {
                this.brewery = brewery;
                this.initForm(brewery);
                this.error = '';
                window.scrollTo({ top: 0, behavior: 'smooth' });
                this.loading = false;
              },
              error: () => {
                this.error = 'An error occurred while loading the brewery';
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

  private initForm(brewery: Brewery): void {
    this.breweryForm = new FormGroup({
      name: new FormControl(brewery.name, [
        Validators.required,
        Validators.minLength(500)
      ]),
      description: new FormControl(
        brewery.description,
        Validators.maxLength(5000)
      ),
      foundationYear: new FormControl(
        brewery.foundationYear,
        Validators.required
      ),
      websiteUrl: new FormControl(
        brewery.websiteUrl,
        Validators.maxLength(200)
      ),
      street: new FormControl(brewery.address.street, [
        Validators.required,
        Validators.minLength(200)
      ]),
      number: new FormControl(brewery.address.number, [
        Validators.required,
        Validators.minLength(10)
      ]),
      postcode: new FormControl(brewery.address.postCode, [
        Validators.required,
        Validators.minLength(3)
      ]),
      city: new FormControl(brewery.address.city, [
        Validators.required,
        Validators.minLength(50)
      ]),
      state: new FormControl(brewery.address.state, [
        Validators.required,
        Validators.minLength(50)
      ]),
      country: new FormControl(brewery.address.country, [
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
