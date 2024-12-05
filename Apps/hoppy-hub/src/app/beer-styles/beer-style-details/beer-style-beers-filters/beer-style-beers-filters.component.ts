import { Component, inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  FormGroup,
  FormControl,
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { Subscription, map } from 'rxjs';
import { BeersParams } from '../../../beers/beers-params';
import { BeersService } from '../../../beers/beers.service';
import { Pagination } from '../../../shared/pagination';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';

@Component({
  selector: 'app-beer-style-beers-filters',
  standalone: true,
  imports: [
    FontAwesomeModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationComponent
  ],
  templateUrl: './beer-style-beers-filters.component.html'
})
export class BeerStyleBeersFiltersComponent implements OnInit, OnDestroy {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;

  private beersService: BeersService = inject(BeersService);
  private route: ActivatedRoute = inject(ActivatedRoute);
  private routeSubscription!: Subscription;

  faX = faX;
  beerStyleId: string = '';
  sortOptions = BeersParams.sortOptions;
  selectedSortOptionIndex: number = 15;
  searchForm: FormGroup = new FormGroup({
    search: new FormControl('')
  });

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap
      .pipe(map(params => params.get('id')))
      .subscribe(beerStyleId => {
        this.beerStyleId = beerStyleId as string;
        this.searchForm.reset();
        this.selectedSortOptionIndex = 15;
      });
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch(): void {
    if (
      this.beersService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.params.searchQuery = this.searchForm.value.search;
      this.beersService.paramsChanged.next(this.params);
    }
  }

  onSort(): void {
    this.params.sortBy = this.sortOptions[this.selectedSortOptionIndex].value;
    this.params.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.beersService.paramsChanged.next(this.params);
  }

  onFiltersClear(): void {
    this.searchForm.reset();
    this.selectedSortOptionIndex = 15;
    this.params = new BeersParams({
      pageSize: 6,
      pageNumber: 1,
      sortBy: 'opinionsCount',
      sortDirection: 1,
      beerStyleId: this.beerStyleId
    });

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
