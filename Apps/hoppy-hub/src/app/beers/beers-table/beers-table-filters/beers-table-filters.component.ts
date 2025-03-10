import { Component, Input, OnInit, inject } from '@angular/core';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { Pagination } from '../../../shared/pagination';
import { BeersParams } from '../../beers-params';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BeersService } from '../../beers.service';
import { ModalService } from '../../../services/modal.service';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';

@Component({
  selector: 'app-beers-table-filters',
  standalone: true,
  imports: [PaginationComponent, ReactiveFormsModule, FontAwesomeModule],
  templateUrl: './beers-table-filters.component.html'
})
export class BeersTableFiltersComponent implements OnInit {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;

  private beersService: BeersService = inject(BeersService);
  private modalService: ModalService = inject(ModalService);

  searchForm!: FormGroup;
  faX = faX;

  ngOnInit(): void {
    this.searchForm = new FormGroup({
      search: new FormControl('')
    });
  }

  onSearch(): void {
    if (
      this.beersService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.params = new BeersParams({
        pageSize: 25,
        pageNumber: 1,
        sortBy: 'releaseDate',
        sortDirection: 1,
        searchQuery: this.searchForm.value.search
      });
      this.beersService.paramsChanged.next(this.params);
    }
  }

  onFiltersModalOpen(): void {
    this.modalService.openModal(new ModalModel(ModalType.BeersFilters));
  }

  onFiltersClear(): void {
    this.searchForm.reset();
    this.params = new BeersParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'releaseDate',
      sortDirection: 1
    });

    if (
      JSON.stringify(this.beersService.paramsChanged.value) !=
      JSON.stringify(this.params)
    ) {
      this.beersService.paramsChanged.next(this.params);
    }
  }
}
