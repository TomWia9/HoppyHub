import { Component, Input, OnInit, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Pagination } from '../../../shared/pagination';
import { BeersService } from '../../../beers/beers.service';
import { BeersParams } from '../../../beers/beers-params';

@Component({
  selector: 'app-brewery-beers-filters',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './brewery-beers-filters.component.html'
})
export class BreweryBeersFiltersComponent implements OnInit {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;

  breweryId: string = '';

  private beersService: BeersService = inject(BeersService);

  searchForm!: FormGroup;

  ngOnInit(): void {
    this.breweryId = this.params.breweryId as string;
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

  onFiltersClear() {
    this.searchForm.reset();
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
}
