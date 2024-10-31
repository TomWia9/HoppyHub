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
import { faX } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-brewery-beers-filters',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    PaginationComponent,
    FontAwesomeModule
  ],
  templateUrl: './brewery-beers-filters.component.html'
})
export class BreweryBeersFiltersComponent implements OnInit, OnDestroy {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;

  private beersService: BeersService = inject(BeersService);
  private route: ActivatedRoute = inject(ActivatedRoute);

  faX = faX;
  breweryId: string = '';
  sortOptions = BeersParams.sortOptions;
  selectedSortOptionIndex: number = 0;
  routeSubscription!: Subscription;
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

  onSearch(): void {
    if (this.searchForm.value.search) {
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
    this.selectedSortOptionIndex = 0;
    this.params = new BeersParams({
      pageSize: 9,
      pageNumber: 1,
      sortBy: 'releaseDate',
      sortDirection: 1,
      breweryId: this.breweryId
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
