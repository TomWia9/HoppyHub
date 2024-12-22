import {
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Observable, Subscription, finalize, forkJoin, map } from 'rxjs';
import { Beer } from '../../../beers/beer.model';
import { BeersService } from '../../../beers/beers.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { BeerStyle } from '../../../beer-styles/beer-style.model';
import { BeerStylesService } from '../../../beer-styles/beer-styles.service';
import { BeerStylesParams } from '../../../beer-styles/beer-styles-params';
import { PagedList } from '../../../shared/paged-list';
import { BreweriesService } from '../../../breweries/breweries.service';
import { BreweriesParams } from '../../../breweries/breweries-params';
import { Brewery } from '../../../breweries/brewery.model';
import { UpsertBeerCommand } from '../../../beers/upsert-beer-command.model';
import { HttpErrorResponse } from '@angular/common/http';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { UpsertBeerImageCommand } from '../../../beers/upsert-beer-image-command.model';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { DeleteBeerModalComponent } from '../delete-beer-modal/delete-beer-modal.component';
import { ModalService } from '../../../services/modal.service';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';
import { BeersParams } from '../../../beers/beers-params';

@Component({
  selector: 'app-beer-edit',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    DeleteBeerModalComponent
  ],
  templateUrl: './beer-edit.component.html'
})
export class BeerEditComponent implements OnInit, OnDestroy {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  private route: ActivatedRoute = inject(ActivatedRoute);
  private router: Router = inject(Router);
  private beersService: BeersService = inject(BeersService);
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private breweriesService: BreweriesService = inject(BreweriesService);
  private alertService: AlertService = inject(AlertService);
  private modalService: ModalService = inject(ModalService);
  private routeSubscription!: Subscription;
  private beerSubscription!: Subscription;
  private beerStylesSubscription!: Subscription;
  private breweriesSubscription!: Subscription;

  beer!: Beer;
  error = '';
  loading = true;
  beerStylesLoading = false;
  breweriesLoading = false;
  beerForm!: FormGroup;
  beerStyles: BeerStyle[] = [];
  breweries: Brewery[] = [];
  selectedImage: File | null = null;
  imageSource: string = '';
  removeImage: boolean = false;
  faX = faX;

  ngOnInit(): void {
    this.fetchAllBeerStyles();
    this.fetchAllBreweries();
    this.getBeer();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files) {
      const allowedTypes = ['image/jpeg', 'image/png'];
      if (!allowedTypes.includes(input.files[0].type)) {
        this.alertService.openAlert(
          AlertType.Error,
          'Only jpg and png files are allowed'
        );
        input.value = '';
        this.selectedImage = null;
        return;
      }
      this.selectedImage = input.files[0];
      const imageUrl = URL.createObjectURL(this.selectedImage);
      this.imageSource = imageUrl;
    }
  }

  onBeerDelete(): void {
    this.modalService.openModal(
      new ModalModel(ModalType.DeleteBeer, {
        beerId: this.beer.id
      })
    );
  }

  onFormSave(): void {
    this.loading = true;
    const operations: Observable<void | string>[] = [];

    if (!this.beerForm.pristine) {
      const upsertBeerCommand = this.beerForm.value as UpsertBeerCommand;
      upsertBeerCommand.id = this.beer.id;
      operations.push(
        this.beersService.updateBeer(this.beer.id, upsertBeerCommand)
      );
    }

    if (this.selectedImage) {
      const upsertBeerImageCommand = new UpsertBeerImageCommand(
        this.beer.id,
        this.selectedImage
      );
      operations.push(
        this.beersService.upsertBeerImage(this.beer.id, upsertBeerImageCommand)
      );
    }

    if (this.removeImage) {
      console.log('operation: DeleteBeerImage');
      operations.push(this.beersService.deleteBeerImage(this.beer.id));
      this.removeImage = false;
    }

    if (operations.length === 0) {
      this.loading = false;
      return;
    }

    forkJoin(operations)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.alertService.openAlert(
            AlertType.Success,
            'Changes saved successfully'
          );
          this.getBeer();
          this.beerChanged();
        },
        error: error => {
          this.handleError(error);
        }
      });
  }

  beerChanged(): void {
    this.beersService.paramsChanged.next(
      new BeersParams({
        pageSize: 10,
        pageNumber: 1,
        sortBy: 'releaseDate',
        sortDirection: 1
      })
    );
  }

  beerDeleted(): void {
    this.beerChanged();
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  private getBeer(): void {
    this.loading = true;
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerId => {
        this.beerSubscription = this.beersService
          .getBeerById(beerId as string)
          .subscribe({
            next: (beer: Beer) => {
              this.beer = beer;
              this.imageSource = `${beer.imageUri}?timestamp=${new Date().getTime()}`;
              this.initForm(beer);
              this.error = '';
              window.scrollTo({ top: 0, behavior: 'smooth' });
              this.loading = false;
            },
            error: () => {
              this.error = 'An error occurred while loading the beer';
              this.loading = false;
            }
          });
      });
  }

  private fetchAllBeerStyles(
    pageNumber: number = 1,
    allBeerStyles: BeerStyle[] = []
  ): void {
    this.beerStylesLoading = true;
    this.beerStylesSubscription = this.beerStylesService
      .getBeerStyles(
        new BeerStylesParams({ pageSize: 50, pageNumber: pageNumber })
      )
      .subscribe({
        next: (beerStyles: PagedList<BeerStyle>) => {
          allBeerStyles.push(...beerStyles.items);
          if (beerStyles.HasNext) {
            this.fetchAllBeerStyles(pageNumber + 1, allBeerStyles);
          } else {
            this.beerStyles = allBeerStyles;
            this.error = '';
            this.beerStylesLoading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading the beer styles';
          this.beerStylesLoading = false;
        }
      });
  }

  private fetchAllBreweries(
    pageNumber: number = 1,
    allBreweries: Brewery[] = []
  ): void {
    this.breweriesLoading = true;
    this.breweriesSubscription = this.breweriesService
      .getBreweries(
        new BreweriesParams({ pageSize: 50, pageNumber: pageNumber })
      )
      .subscribe({
        next: (breweries: PagedList<Brewery>) => {
          allBreweries.push(...breweries.items);
          if (breweries.HasNext) {
            this.fetchAllBreweries(pageNumber + 1, allBreweries);
          } else {
            this.breweries = allBreweries;
            this.error = '';
            this.breweriesLoading = false;
          }
        },
        error: () => {
          this.error = 'An error occurred while loading the breweries';
          this.breweriesLoading = false;
        }
      });
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

  private initForm(beer: Beer): void {
    this.beerForm = new FormGroup({
      name: new FormControl(beer.name, [
        Validators.required,
        Validators.minLength(2)
      ]),
      description: new FormControl(beer.description, Validators.maxLength(500)),
      beerStyleId: new FormControl(beer.beerStyle.id, Validators.required),
      breweryId: new FormControl(beer.brewery.id, Validators.required),
      releaseDate: new FormControl(beer.releaseDate, Validators.required),
      alcoholByVolume: new FormControl(beer.alcoholByVolume, [
        Validators.min(0),
        Validators.max(100)
      ]),
      blg: new FormControl(beer.blg, [Validators.min(0), Validators.max(100)]),
      ibu: new FormControl(beer.ibu, [Validators.min(0), Validators.max(120)]),
      composition: new FormControl(beer.composition, Validators.maxLength(1000))
    });
  }

  ngOnDestroy(): void {
    if (this.beerSubscription) {
      this.beerSubscription.unsubscribe();
    }
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.beerStylesSubscription) {
      this.beerStylesSubscription.unsubscribe();
    }
    if (this.breweriesSubscription) {
      this.breweriesSubscription.unsubscribe();
    }
    if (this.imageSource) {
      URL.revokeObjectURL(this.imageSource);
    }
  }
}
