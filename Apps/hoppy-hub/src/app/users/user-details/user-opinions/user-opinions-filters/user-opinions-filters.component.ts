import { Component, inject, Input, OnInit } from '@angular/core';
import { OpinionsParams } from '../../../../opinions/opinions-params';
import { OpinionsService } from '../../../../opinions/opinions.service';
import {
  FormsModule,
  ReactiveFormsModule,
  FormGroup,
  FormControl
} from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { PaginationComponent } from '../../../../shared-components/pagination/pagination.component';
import { Pagination } from '../../../../shared/pagination';

@Component({
  selector: 'app-user-opinions-filters',
  standalone: true,
  imports: [
    FontAwesomeModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationComponent
  ],
  templateUrl: './user-opinions-filters.component.html'
})
export class UserOpinionsFiltersComponent implements OnInit {
  @Input({ required: true }) opinionsParams!: OpinionsParams;
  @Input({ required: true }) paginationData!: Pagination;
  @Input({ required: true }) userId!: string;

  private opinionsService: OpinionsService = inject(OpinionsService);
  faX = faX;
  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });
  selectedSortOptionIndex: number = 0;
  sortOptions = OpinionsParams.sortOptions;

  ngOnInit(): void {
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch(): void {
    if (
      this.opinionsService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.opinionsParams.searchQuery = this.searchForm.value.search;
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

  onFiltersClear(): void {
    this.searchForm.reset();
    this.selectedSortOptionIndex = 0;
    this.opinionsParams = new OpinionsParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'created',
      sortDirection: 1,
      userId: this.userId
    });

    if (
      JSON.stringify(this.opinionsService.paramsChanged.value) !=
      JSON.stringify(this.opinionsParams)
    ) {
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }
}
