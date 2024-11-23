import { Component, inject, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OpinionsParams } from '../../../../../opinions/opinions-params';
import { OpinionsService } from '../../../../../opinions/opinions.service';
import { Pagination } from '../../../../../shared/pagination';
import { PaginationComponent } from '../../../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-beer-opinions-list-filters',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, PaginationComponent],
  templateUrl: './beer-opinions-list-filters.component.html'
})
export class BeerOpinionsListFiltersComponent {
  @Input({ required: true }) opinionsParams!: OpinionsParams;
  @Input({ required: true }) paginationData!: Pagination;

  private opinionsService: OpinionsService = inject(OpinionsService);

  selectedSortOptionIndex: number = 0;
  sortOptions = OpinionsParams.sortOptions;
  onlyOpinionsWithImages: boolean = false;
  onlyOpinionsWithComments: boolean = false;

  onShowOnlyOpinionsWithImages(): void {
    if (
      this.opinionsService.paramsChanged.value.hasImage !==
      this.onlyOpinionsWithImages
    ) {
      this.opinionsParams.hasImage =
        this.onlyOpinionsWithImages == false
          ? undefined
          : this.onlyOpinionsWithImages;
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  onShowOnlyOpinionsWithComments(): void {
    if (
      this.opinionsService.paramsChanged.value.hasComment !==
      this.onlyOpinionsWithComments
    ) {
      this.opinionsParams.hasComment =
        this.onlyOpinionsWithComments == false
          ? undefined
          : this.onlyOpinionsWithComments;
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  onSort(): void {
    this.opinionsParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.opinionsParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }
}
