import { Component, Input, OnDestroy, OnInit, inject } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';
import { Pagination } from '../../../shared/pagination';
import { BeersService } from '../../../beers/beers.service';
import { BeersParams } from '../../../beers/beers-params';
import { Subscription, map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-brewery-beers-filters',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, PaginationComponent],
  templateUrl: './brewery-beers-filters.component.html'
})
export class BreweryBeersFiltersComponent implements OnInit, OnDestroy {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;

  breweryId: string = '';
  sortOptions = [
    {
      label: 'Release date (New to Old)',
      value: 'ReleaseDate',
      direction: 0
    },
    {
      label: 'Release date (Old to New)',
      value: 'ReleaseDate',
      direction: 1
    },
    { label: 'Name (A to Z)', value: 'Name', direction: 0 },
    { label: 'Name (Z to A)', value: 'Name', direction: 1 },
    { label: 'Beer style (A to Z)', value: 'BeerStyle', direction: 0 },
    { label: 'Beer style (Z to A)', value: 'BeerStyle', direction: 1 },
    {
      label: 'Alcohol by volume (Low to High)',
      value: 'AlcoholByVolume',
      direction: 0
    },
    {
      label: 'Alcohol by volume (High to Low)',
      value: 'AlcoholByVolume',
      direction: 1
    },
    { label: 'BLG (Low to High)', value: 'BLG', direction: 0 },
    { label: 'BLG (High to Low)', value: 'BLG', direction: 1 },
    { label: 'IBU (Low to High)', value: 'IBU', direction: 0 },
    { label: 'IBU (High to Low)', value: 'IBU', direction: 1 },
    { label: 'Rating (Low to High)', value: 'Rating', direction: 0 },
    { label: 'Rating (High to Low)', value: 'Rating', direction: 1 },
    {
      label: 'Opinions count (Low to High)',
      value: 'OpinionsCount',
      direction: 0
    },
    {
      label: 'Opinions count (High to Low)',
      value: 'OpinionsCount',
      direction: 1
    },
    {
      label: 'Favorites count (Low to High)',
      value: 'FavoritesCount',
      direction: 0
    },
    {
      label: 'Favorites count (High to Low)',
      value: 'FavoritesCount',
      direction: 1
    }
  ];

  selectedSortOptionIndex: number = 0;
  routeSubscription!: Subscription;

  private beersService: BeersService = inject(BeersService);
  private route: ActivatedRoute = inject(ActivatedRoute);

  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(breweryId => {
        this.breweryId = breweryId as string;
        this.searchForm.reset();
        this.selectedSortOptionIndex = 0;
      });
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch() {
    if (this.searchForm.value.search) {
      this.params.searchQuery = this.searchForm.value.search;
      this.beersService.paramsChanged.next(this.params);
    }
  }

  onSort() {
    this.params.sortBy = this.sortOptions[this.selectedSortOptionIndex].value;
    this.params.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.beersService.paramsChanged.next(this.params);
  }

  onFiltersClear() {
    this.searchForm.reset();
    this.selectedSortOptionIndex = 0;
    this.params = new BeersParams(
      9,
      1,
      'ReleaseDate',
      1,
      undefined,
      undefined,
      this.breweryId
    );

    if (
      JSON.stringify(this.beersService.paramsChanged.value) !=
      JSON.stringify(this.params)
    ) {
      this.beersService.paramsChanged.next(this.params);
    }
  }

  ngOnDestroy(): void {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
  }
}
