import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  inject
} from '@angular/core';
import { BeersService } from '../../beers/beers.service';
import { BeersParams } from '../../beers/beers-params';
import { Pagination } from '../../shared/pagination';
import { BreweriesParams } from '../../breweries/breweries-params';
import { BreweriesService } from '../../breweries/breweries.service';
import { OpinionsParams } from '../../opinions/opinions-params';
import { OpinionsService } from '../../opinions/opinions.service';
import { FavoritesParams } from '../../favorites/favorites-params';
import { FavoritesService } from '../../favorites/favorites.service';
import { BeerStylesService } from '../../beer-styles/beer-styles.service';
import { BeerStylesParams } from '../../beer-styles/beer-styles-params';
import { UsersParams } from '../../users/users-params';
import { UsersService } from '../../users/users.service';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html'
})
export class PaginationComponent implements OnInit {
  @Input({ required: true }) params!:
    | BeersParams
    | BreweriesParams
    | BeerStylesParams
    | OpinionsParams
    | FavoritesParams
    | UsersParams;
  @Input({ required: true }) paginationData!: Pagination;
  @Input() size: string = 'lg';
  @Output() scrollTo = new EventEmitter<void>();

  paginationSize = 'lg';

  private beersService: BeersService = inject(BeersService);
  private breweriesService: BreweriesService = inject(BreweriesService);
  private beerStylesService: BeerStylesService = inject(BeerStylesService);
  private opinionsService: OpinionsService = inject(OpinionsService);
  private favoritesService: FavoritesService = inject(FavoritesService);
  private usersService: UsersService = inject(UsersService);

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
    this.scrollTo.emit();
  }

  private getData(): void {
    if (this.params instanceof BeersParams) {
      this.beersService.paramsChanged.next(this.params);
    }
    if (this.params instanceof BreweriesParams) {
      this.breweriesService.paramsChanged.next(this.params);
    }
    if (this.params instanceof BeerStylesParams) {
      this.beerStylesService.paramsChanged.next(this.params);
    }
    if (this.params instanceof OpinionsParams) {
      this.opinionsService.paramsChanged.next(this.params);
    }
    if (this.params instanceof FavoritesParams) {
      this.favoritesService.paramsChanged.next(this.params);
    }
    if (this.params instanceof UsersParams) {
      this.usersService.paramsChanged.next(this.params);
    }
  }
}
