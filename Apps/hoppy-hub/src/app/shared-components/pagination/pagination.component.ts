import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { Pagination } from '../../shared/pagination';
import { BeersService } from '../../beers/beers.service';
import { BeersParams } from '../../beers/beers-params';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css'
})
export class PaginationComponent {
  @Input({ required: true }) paginationData!: Pagination;
  @Input({ required: true }) params!: BeersParams;

  private beersService: BeersService = inject(BeersService);

  onChangePage(pageNumber: number): void {
    this.params.pageNumber = pageNumber;
    this.getData();
  }

  onNextPage(): void {
    this.params.pageNumber++;
    this.getData();
  }

  onPreviousPage(): void {
    this.params.pageNumber--;
    this.getData();
  }

  private getData(): void {
    if (this.params instanceof BeersParams) {
      // this.beersService.paramsChanged.next(params);
    }
  }
}
