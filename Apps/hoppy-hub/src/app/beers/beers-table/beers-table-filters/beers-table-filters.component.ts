import { Component, Input, OnInit, inject } from '@angular/core';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { Pagination } from '../../../shared/pagination';
import { BeersParams } from '../../beers-params';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BeersService } from '../../beers.service';

@Component({
  selector: 'app-beers-table-filters',
  standalone: true,
  imports: [PaginationComponent, ReactiveFormsModule],
  templateUrl: './beers-table-filters.component.html',
  styleUrl: './beers-table-filters.component.css'
})
export class BeersTableFiltersComponent implements OnInit {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;

  private beersService: BeersService = inject(BeersService);

  searchForm!: FormGroup;

  ngOnInit(): void {
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
    this.params = new BeersParams(25, 1, 'ReleaseDate', 1);
    this.beersService.paramsChanged.next(this.params);
  }
}
