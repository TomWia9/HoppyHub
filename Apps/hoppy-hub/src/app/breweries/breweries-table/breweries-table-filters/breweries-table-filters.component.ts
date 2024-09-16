import { Component, Input, OnInit, inject } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ModalService, ModalType } from '../../../services/modal.service';
import { Pagination } from '../../../shared/pagination';
import { BreweriesParams } from '../../breweries-params';
import { BreweriesService } from '../../breweries.service';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faX } from '@fortawesome/free-solid-svg-icons';

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

  onSearch() {
    if (this.searchForm.value.search) {
      this.params = new BreweriesParams(
        25,
        1,
        'Name',
        1,
        this.searchForm.value.search
      );
      this.breweriesService.paramsChanged.next(this.params);
    }
  }

  onFiltersModalOpen() {
    this.modalService.openModal(ModalType.BreweriesFilters);
  }

  onFiltersClear() {
    this.searchForm.reset();
    this.params = new BreweriesParams(25, 1, 'Name', 1);

    if (
      JSON.stringify(this.breweriesService.paramsChanged.value) !=
      JSON.stringify(this.params)
    ) {
      this.breweriesService.paramsChanged.next(this.params);
    }
  }
}
