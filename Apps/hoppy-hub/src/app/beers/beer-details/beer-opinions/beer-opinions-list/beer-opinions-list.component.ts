import {
  Component,
  ElementRef,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { OpinionsService } from '../../../../opinions/opinions.service';
import { DataHelper } from '../../../../shared/data-helper';
import { Subscription } from 'rxjs';
import { Opinion } from '../../../../opinions/opinion.model';
import { OpinionsParams } from '../../../../opinions/opinions-params';
import { PagedList } from '../../../../shared/paged-list';
import { Pagination } from '../../../../shared/pagination';
import { PaginationComponent } from '../../../../shared-components/pagination/pagination.component';
import { FormsModule } from '@angular/forms';
import { OpinionComponent } from '../../../../opinions/opinion/opinion.component';
import { Beer } from '../../../beer.model';
import { BeerOpinionsListFiltersComponent } from './beer-opinions-list-filters/beer-opinions-list-filters.component';

@Component({
  selector: 'app-beer-opinions-list',
  standalone: true,
  imports: [
    PaginationComponent,
    FormsModule,
    OpinionComponent,
    BeerOpinionsListFiltersComponent
  ],
  templateUrl: './beer-opinions-list.component.html'
})
export class BeerOpinionsListComponent
  extends DataHelper
  implements OnChanges, OnDestroy
{
  @Input({ required: true }) beer!: Beer;
  @ViewChild('#showOpinionsButton') showOpinionsButton!: ElementRef;
  private opinionsService: OpinionsService = inject(OpinionsService);

  sortOptions = OpinionsParams.sortOptions;

  opinionsParamsSubscription!: Subscription;
  getOpinionsSubscription!: Subscription;
  selectedSortOptionIndex: number = 0;
  opinionsParams = new OpinionsParams({
    pageSize: 10,
    pageNumber: 1,
    sortBy: 'created',
    sortDirection: 1
  });
  opinions: PagedList<Opinion> | undefined;
  paginationData!: Pagination;
  error = '';
  opinionsLoading = true;
  showOpinions = false;

  ngOnChanges(): void {
    this.refreshOpinions();
  }

  refreshOpinions(): void {
    this.opinionsParams.beerId = this.beer.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onSort(): void {
    this.opinionsParams.pageNumber = 1;
    this.opinionsParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.opinionsParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onFiltersClear(): void {
    this.selectedSortOptionIndex = 0;
    this.opinionsParams = new OpinionsParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'created',
      sortDirection: 1
    });

    if (
      JSON.stringify(this.opinionsService.paramsChanged.value) !=
      JSON.stringify(this.opinionsParams)
    ) {
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  toggleOpinions(): void {
    if (this.showOpinions) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    } else if (!this.opinions) {
      this.opinionsParamsSubscription =
        this.opinionsService.paramsChanged.subscribe(
          (params: OpinionsParams) => {
            this.opinionsParams = params;
            this.opinionsParams.beerId = this.beer.id;
            this.getOpinions();
          }
        );
    }
    this.showOpinions = !this.showOpinions;
  }

  scrollToTop(): void {
    const elementPosition =
      this.showOpinionsButton.nativeElement.getBoundingClientRect().top +
      window.scrollY;

    window.scrollTo({
      top: elementPosition,
      behavior: 'smooth'
    });
  }

  private getOpinions(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(this.opinionsParams)
      .subscribe({
        next: (opinions: PagedList<Opinion>) => {
          this.opinionsLoading = true;
          this.opinions = opinions;
          this.paginationData = this.getPaginationData(opinions);
          this.error = '';
          this.opinionsLoading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the opinions';

          if (error.error && error.error.errors) {
            const errorMessage = this.getValidationErrorMessage(
              error.error.errors
            );
            this.error += errorMessage;
          }

          this.opinionsLoading = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.getOpinionsSubscription) {
      this.getOpinionsSubscription.unsubscribe();
    }
    if (this.opinionsParamsSubscription) {
      this.opinionsParamsSubscription.unsubscribe();
    }
  }
}
