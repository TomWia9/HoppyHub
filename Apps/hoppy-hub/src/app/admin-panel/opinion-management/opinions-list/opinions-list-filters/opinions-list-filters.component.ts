import { Component, inject, Input } from '@angular/core';
import { OpinionsParams } from '../../../../opinions/opinions-params';
import { OpinionsService } from '../../../../opinions/opinions.service';
import { Pagination } from '../../../../shared/pagination';
import { PaginationComponent } from '../../../../shared-components/pagination/pagination.component';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-opinions-list-filters',
  standalone: true,
  imports: [
    PaginationComponent,
    FormsModule,
    FontAwesomeModule,
    ReactiveFormsModule
  ],
  templateUrl: './opinions-list-filters.component.html'
})
export class OpinionsListFiltersComponent {
  @Input({ required: true }) opinionsParams!: OpinionsParams;
  @Input({ required: true }) paginationData!: Pagination;

  private opinionsService: OpinionsService = inject(OpinionsService);

  selectedSortOptionIndex: number = 0;
  sortOptions = OpinionsParams.sortOptions;
  onlyOpinionsWithImages: boolean = false;
  onlyOpinionsWithComments: boolean = false;
  faX = faX;
  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });

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

  onSearch(): void {
    if (
      this.opinionsService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.opinionsParams.searchQuery = this.searchForm.value.search;
      this.opinionsParams.pageSize = 10;
      this.opinionsParams.pageNumber = 1;
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  onFiltersClear(): void {
    this.searchForm.reset();
    this.onlyOpinionsWithImages = false;
    this.onlyOpinionsWithComments = false;
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
}
