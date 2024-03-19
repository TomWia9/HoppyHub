import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, inject } from '@angular/core';
import { BeersService } from '../../beers/beers.service';
import { BeersParams } from '../../beers/beers-params';
import { Pagination } from '../../shared/pagination';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html'
})
export class PaginationComponent implements OnInit {
  @Input({ required: true }) params!: BeersParams;
  @Input({ required: true }) paginationData!: Pagination;
  @Input() size: string = 'lg';

  paginationSize = 'lg';

  private beersService: BeersService = inject(BeersService);

  ngOnInit(): void {
    switch (this.size) {
      case 'xs':
        this.paginationSize = 'btn-xs';
        break;
      case 'sm':
        this.paginationSize = 'btn-sm';
        break;
      case 'md':
        this.paginationSize = 'btn-md';
        break;
      default:
        this.paginationSize = 'btn-lg';
    }
  }

  onChangePage(pageNumber: number): void {
    this.params.pageNumber = pageNumber;
    this.getData();
  }

  private getData(): void {
    if (this.params instanceof BeersParams) {
      this.beersService.paramsChanged.next(this.params);
    }
  }
}
