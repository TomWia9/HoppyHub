import { Component, Input, OnInit, inject } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ModalService } from '../../../services/modal.service';
import { Pagination } from '../../../shared/pagination';
import { BreweriesParams } from '../../breweries-params';
import { BreweriesService } from '../../breweries.service';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { ModalType } from '../../../shared/model-type';
import { ModalModel } from '../../../shared/modal-model';

@Component({
  selector: 'app-breweries-table-filters',
  standalone: true,
  templateUrl: './breweries-table-filters.component.html',
  imports: [PaginationComponent, ReactiveFormsModule, FontAwesomeModule]
})
export class BreweriesTableFiltersComponent implements OnInit {
  @Input({ required: true }) params!: BreweriesParams;
  @Input({ required: true }) paginationData!: Pagination;

  private breweriesService: BreweriesService = inject(BreweriesService);
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
      this.breweriesService.paramsChanged.value.searchQuery !==
      this.searchForm.value.search
    ) {
      this.params = new BreweriesParams({
        pageSize: 25,
        pageNumber: 1,
        sortBy: 'Name',
        sortDirection: 1,
        searchQuery: this.searchForm.value.search
      });
      this.breweriesService.paramsChanged.next(this.params);
    }
  }

  onFiltersModalOpen(): void {
    this.modalService.openModal(new ModalModel(ModalType.BreweriesFilters));
  }

  onFiltersClear(): void {
    this.searchForm.reset();
    this.params = new BreweriesParams({
      pageSize: 25,
      pageNumber: 1,
      sortBy: 'Name',
      sortDirection: 1
    });

    if (
      JSON.stringify(this.breweriesService.paramsChanged.value) !=
      JSON.stringify(this.params)
    ) {
      this.breweriesService.paramsChanged.next(this.params);
    }
  }
}
