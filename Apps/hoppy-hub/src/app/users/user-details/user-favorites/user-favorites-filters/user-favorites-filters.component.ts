import { Component, inject, Input, OnInit } from '@angular/core';
import { Pagination } from '../../../../shared/pagination';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';
import { FavoritesParams } from '../../../../favorites/favorites-params';
import { FavoritesService } from '../../../../favorites/favorites.service';
import { PaginationComponent } from '../../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-user-favorites-filters',
  standalone: true,
  imports: [
    FontAwesomeModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationComponent
  ],
  templateUrl: './user-favorites-filters.component.html'
})
export class UserFavoritesFiltersComponent implements OnInit {
  @Input({ required: true }) favoriteBeersParams!: FavoritesParams;
  @Input({ required: true }) paginationData!: Pagination;
  @Input({ required: true }) userId!: string;

  private favoritesService: FavoritesService = inject(FavoritesService);
  faX = faX;
  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });
  selectedSortIndex: number = 0;
  sortOptions = FavoritesParams.sortOptions;

  ngOnInit(): void {
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch(): void {
    if (
      this.favoritesService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.favoriteBeersParams.searchQuery = this.searchForm.value.search;
      this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
    }
  }

  onSort(): void {
    this.favoriteBeersParams.sortBy =
      this.sortOptions[this.selectedSortIndex].value;
    this.favoriteBeersParams.sortDirection =
      this.sortOptions[this.selectedSortIndex].direction;
    this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
  }

  onFiltersClear(): void {
    this.searchForm.reset();
    this.selectedSortIndex = 0;
    this.favoriteBeersParams = new FavoritesParams({
      pageSize: 6,
      pageNumber: 1,
      sortBy: 'LastModified',
      sortDirection: 1,
      userId: this.userId
    });

    if (
      JSON.stringify(this.favoritesService.paramsChanged.value) !=
      JSON.stringify(this.favoriteBeersParams)
    ) {
      this.favoritesService.paramsChanged.next(this.favoriteBeersParams);
    }
  }
}
