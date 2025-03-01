import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import { BeersService } from '../beers.service';
import { Beer } from '../beer.model';
import { PagedList } from '../../shared/paged-list';
import { Subject, takeUntil, tap } from 'rxjs';
import { BeersParams } from '../beers-params';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { PaginationComponent } from '../../shared-components/pagination/pagination.component';
import { Pagination } from '../../shared/pagination';
import { BeersTableFiltersComponent } from './beers-table-filters/beers-table-filters.component';
import { RouterModule } from '@angular/router';
import { DataHelper } from '../../shared/data-helper';

@Component({
  selector: 'app-beers-table',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    PaginationComponent,
    BeersTableFiltersComponent,
    RouterModule
  ],
  templateUrl: './beers-table.component.html'
})
export class BeersTableComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('topSection') topSection!: ElementRef;
  private beersService: BeersService = inject(BeersService);
  private destroy$ = new Subject<void>();

  beersParams = new BeersParams({
    pageSize: 10,
    pageNumber: 1,
    sortBy: 'releaseDate',
    sortDirection: 1
  });
  beers: PagedList<Beer> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;

  ngOnInit(): void {
    this.beersService.paramsChanged.next(this.beersParams);
    this.beersService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap((params: BeersParams) => {
          this.beersParams = params;
          this.getBeers();
        })
      )
      .subscribe();
  }

  private getBeers(): void {
    this.beersService
      .getBeers(this.beersParams)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: (beers: PagedList<Beer>) => {
            this.beers = beers;
            this.beers.items.forEach(beer => {
              beer.imageUri = `${beer.imageUri}?timestamp=${new Date().getTime()}`;
            });
            this.paginationData = this.getPaginationData(beers);
            this.error = '';
          },
          error: error => {
            this.error = 'An error occurred while loading the beers';

            if (error.error && error.error.errors) {
              const errorMessage = this.getValidationErrorMessage(
                error.error.errors
              );
              this.error += errorMessage;
            }
          },
          complete: () => {
            this.loading = false;
          }
        })
      )
      .subscribe();
  }

  scrollToTop(): void {
    const elementPosition =
      this.topSection.nativeElement.getBoundingClientRect().top +
      window.scrollY;

    window.scrollTo({
      top: elementPosition,
      behavior: 'smooth'
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
