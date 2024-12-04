import {
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { Subscription } from 'rxjs';
import { PagedList } from '../../shared/paged-list';
import { Pagination } from '../../shared/pagination';
import { BeerStylesService } from '../beer-styles.service';
import { DataHelper } from '../../shared/data-helper';
import { BeerStyle } from '../beer-style.model';
import { BeerStylesParams } from '../beer-styles-params';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { RouterModule } from '@angular/router';
import { PaginationComponent } from '../../shared-components/pagination/pagination.component';
import { BeerStylesTableFiltersComponent } from './beer-styles-table-filters/beer-styles-table-filters.component';

@Component({
  selector: 'app-beer-styles-table',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    PaginationComponent,
    BeerStylesTableFiltersComponent
  ],
  templateUrl: './beer-styles-table.component.html'
})
export class BeerStylesTableComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  @ViewChild('topSection') topSection!: ElementRef;
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private beerStylesParamsSubscription!: Subscription;
  private getBeerStylesSubscription!: Subscription;

  beerStylesParams = new BeerStylesParams({
    pageSize: 15,
    pageNumber: 1
  });
  beerStyles: PagedList<BeerStyle> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;

  ngOnInit(): void {
    this.beerStylesService.paramsChanged.next(this.beerStylesParams);
    this.beerStylesParamsSubscription =
      this.beerStylesService.paramsChanged.subscribe(
        (params: BeerStylesParams) => {
          this.beerStylesParams = params;
          this.getBeerStyles();
        }
      );
  }

  private getBeerStyles(): void {
    this.getBeerStylesSubscription = this.beerStylesService
      .getBeerStyles(this.beerStylesParams)
      .subscribe({
        next: (beerStyles: PagedList<BeerStyle>) => {
          this.loading = true;
          this.beerStyles = beerStyles;
          this.paginationData = this.getPaginationData(beerStyles);
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the beer styles';

          if (error.error && error.error.errors) {
            const errorMessage = this.getValidationErrorMessage(
              error.error.errors
            );
            this.error += errorMessage;
          }

          this.loading = false;
        }
      });
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
    if (this.beerStylesParamsSubscription) {
      this.beerStylesParamsSubscription.unsubscribe();
    }
    if (this.getBeerStylesSubscription) {
      this.getBeerStylesSubscription.unsubscribe();
    }
  }
}
