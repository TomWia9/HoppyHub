import { Component, Input } from '@angular/core';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { Pagination } from '../../../shared/pagination';
import { BeersParams } from '../../beers-params';

@Component({
  selector: 'app-beers-table-filters',
  standalone: true,
  imports: [PaginationComponent],
  templateUrl: './beers-table-filters.component.html',
  styleUrl: './beers-table-filters.component.css'
})
export class BeersTableFiltersComponent {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;
}
