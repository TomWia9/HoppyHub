import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnDestroy,
  Output
} from '@angular/core';
import { Beer } from '../beer.model';
import { RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { FavoritesService } from '../../favorites/favorites.service';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-beer-card',
  standalone: true,
  imports: [RouterModule, FontAwesomeModule],
  templateUrl: './beer-card.component.html'
})
export class BeerCardComponent implements OnDestroy {
  @Input({ required: true }) beer!: Beer;
  @Input() showBrewery: boolean = false;
  @Input() editMode: boolean = false;
  @Output() favoriteDeleted = new EventEmitter<void>();
  private favoritesService = inject(FavoritesService);
  private deleteFavoriteSubscription!: Subscription;
  faX = faX;

  onFavoriteDelete(): void {
    this.deleteFavoriteSubscription = this.favoritesService
      .deleteFavorite(this.beer.id)
      .subscribe({
        next: () => {
          this.favoriteDeleted.emit();
        }
      });
  }

  ngOnDestroy(): void {
    if (this.deleteFavoriteSubscription) {
      this.deleteFavoriteSubscription.unsubscribe();
    }
  }
}
