import { Component, inject, Input, OnInit } from '@angular/core';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  FormsModule
} from '@angular/forms';
import { Pagination } from '../../../shared/pagination';
import { BeerStylesParams } from '../../beer-styles-params';
import { BeerStylesService } from '../../beer-styles.service';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-beer-styles-table-filters',
  standalone: true,
  imports: [PaginationComponent, ReactiveFormsModule, FormsModule],
  templateUrl: './beer-styles-table-filters.component.html'
})
export class BeerStylesTableFiltersComponent implements OnInit {
  @Input({ required: true }) params!: BeerStylesParams;
  @Input({ required: true }) paginationData!: Pagination;

  private beerStylesService: BeerStylesService = inject(BeerStylesService);

  searchForm!: FormGroup;
  selectedSortOptionIndex: number = 0;
  sortOptions = BeerStylesParams.sortOptions;

  ngOnInit(): void {
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch(): void {
    if (
      this.beerStylesService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.params.searchQuery = this.searchForm.value.search;
      this.beerStylesService.paramsChanged.next(this.params);
    }
  }

  onSort(): void {
    this.params.sortBy = this.sortOptions[this.selectedSortOptionIndex].value;
    this.params.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.beerStylesService.paramsChanged.next(this.params);
  }
}
