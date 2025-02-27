import {
  Component,
  ElementRef,
  inject,
  OnChanges,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { OpinionsListFiltersComponent } from './opinions-list-filters/opinions-list-filters.component';
import { Subject, takeUntil, tap } from 'rxjs';
import { Opinion } from '../../../opinions/opinion.model';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { OpinionsService } from '../../../opinions/opinions.service';
import { DataHelper } from '../../../shared/data-helper';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';
import { DeleteOpinionModalComponent } from '../../../opinions/delete-opinion-modal/delete-opinion-modal.component';
import { UpsertOpinionModalComponent } from '../../../opinions/upsert-opinion-modal/upsert-opinion-modal.component';

@Component({
  selector: 'app-opinions-list',
  standalone: true,
  imports: [
    OpinionsListFiltersComponent,
    LoadingSpinnerComponent,
    PaginationComponent,
    OpinionComponent,
    DeleteOpinionModalComponent,
    UpsertOpinionModalComponent
  ],
  templateUrl: './opinions-list.component.html'
})
export class OpinionsListComponent
  extends DataHelper
  implements OnInit, OnChanges, OnDestroy
{
  @ViewChild('topOfList') topOfList!: ElementRef;

  private opinionsService: OpinionsService = inject(OpinionsService);
  private destroy$ = new Subject<void>();

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

  ngOnInit(): void {
    this.opinionsService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap((params: OpinionsParams) => {
          this.opinionsParams = params;
          this.getOpinions();
        })
      )
      .subscribe();
  }

  ngOnChanges(): void {
    this.refreshOpinions();
  }

  refreshOpinions(): void {
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  scrollToTop(): void {
    const elementPosition =
      this.topOfList.nativeElement.getBoundingClientRect().top + window.scrollY;

    window.scrollTo({
      top: elementPosition,
      behavior: 'smooth'
    });
  }

  private getOpinions(): void {
    this.opinionsService
      .getOpinions(this.opinionsParams)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: (opinions: PagedList<Opinion>) => {
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
        })
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
