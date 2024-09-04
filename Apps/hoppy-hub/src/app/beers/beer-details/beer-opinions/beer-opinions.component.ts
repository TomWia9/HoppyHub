import {
  Component,
  ElementRef,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { OpinionsService } from '../../../opinions/opinions.service';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { Opinion } from '../../../opinions/opinion.model';
import { Subscription } from 'rxjs';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { Beer } from '../../beer.model';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-beer-opinions',
  standalone: true,
  imports: [OpinionComponent, FormsModule, PaginationComponent],
  templateUrl: './beer-opinions.component.html'
})
export class BeerOpinionsComponent implements OnInit, OnChanges, OnDestroy {
  @Input({ required: true }) beer!: Beer;
  @ViewChild('opinionsSection') opinionsSection!: ElementRef;

  private opinionsService: OpinionsService = inject(OpinionsService);

  sortOptions = [
    {
      label: 'Created (New to Old)',
      value: 'Created',
      direction: 0
    },
    {
      label: 'Created (Old to New)',
      value: 'Created',
      direction: 1
    },
    { label: 'Rating (High to Low)', value: 'Rating', direction: 1 },
    { label: 'Rating (Low to High)', value: 'Rating', direction: 0 }
  ];
  selectedSortOptionIndex: number = 0;
  opinionsParams = new OpinionsParams(10, 1, 'created', 1);
  opinions: PagedList<Opinion> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;
  opinionsParamsSubscription!: Subscription;
  getOpinionsSubscription!: Subscription;
  showOpinions = false;

  ngOnInit(): void {
    this.opinionsParamsSubscription =
      this.opinionsService.paramsChanged.subscribe((params: OpinionsParams) => {
        this.opinionsParams = params;
        this.getOpinions();
      });
  }

  ngOnChanges() {
    this.opinionsParams.beerId = this.beer.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onSort() {
    this.opinionsParams.pageNumber = 1;
    this.opinionsParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.opinionsParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onFiltersClear() {
    this.selectedSortOptionIndex = 0;
    this.opinionsParams = new OpinionsParams(10, 1, 'created', 1);

    if (
      JSON.stringify(this.opinionsService.paramsChanged.value) !=
      JSON.stringify(this.opinionsParams)
    ) {
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  toggleOpinions() {
    if (this.showOpinions) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
    this.showOpinions = !this.showOpinions;
  }

  scrollToTop() {
    const elementPosition =
      this.opinionsSection.nativeElement.getBoundingClientRect().top +
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
          this.loading = true;
          this.opinions = opinions;
          this.paginationData = this.getPaginationData();
          this.error = '';
          this.loading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the opinions';

          if (error.error && error.error.errors) {
            const errorMessage = this.getErrorMessage(error.error.errors);
            this.error += errorMessage;
          }

          this.loading = false;
        }
      });
  }

  private getPaginationData(): Pagination {
    if (this.opinions) {
      return {
        CurrentPage: this.opinions.CurrentPage,
        HasNext: this.opinions.HasNext,
        HasPrevious: this.opinions.HasPrevious,
        TotalPages: this.opinions.TotalPages,
        TotalCount: this.opinions.TotalCount
      };
    }

    return {
      CurrentPage: 0,
      HasNext: false,
      HasPrevious: false,
      TotalPages: 0,
      TotalCount: 0
    };
  }

  private getErrorMessage(array: { [key: string]: string }[]): string {
    if (array.length === 0) {
      return '';
    }

    const firstObject = Object.values(array)[0];
    const errorMessage = Object.values(firstObject)[0];

    if (!errorMessage) {
      return '';
    }

    return ': ' + errorMessage;
  }

  ngOnDestroy(): void {
    this.getOpinionsSubscription.unsubscribe();
    this.opinionsParamsSubscription.unsubscribe();
  }
}
