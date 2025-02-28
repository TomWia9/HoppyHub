import { Component, inject, Input, OnInit } from '@angular/core';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  FormsModule
} from '@angular/forms';
import { Pagination } from '../../../../shared/pagination';
import { UsersParams } from '../../../../users/users-params';
import { UsersService } from '../../../../users/users.service';
import { PaginationComponent } from '../../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-users-list-filters',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, PaginationComponent],
  templateUrl: './users-list-filters.component.html'
})
export class UsersListFiltersComponent implements OnInit {
  @Input({ required: true }) params!: UsersParams;
  @Input({ required: true }) paginationData!: Pagination;

  private usersService: UsersService = inject(UsersService);

  searchForm!: FormGroup;
  selectedSortOptionIndex: number = 0;
  sortOptions = UsersParams.sortOptions;

  ngOnInit(): void {
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch(): void {
    if (
      this.usersService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.params.searchQuery = this.searchForm.value.search;
      this.usersService.paramsChanged.next(this.params);
    }
  }

  onSort(): void {
    this.params.sortBy = this.sortOptions[this.selectedSortOptionIndex].value;
    this.params.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.usersService.paramsChanged.next(this.params);
  }
}
