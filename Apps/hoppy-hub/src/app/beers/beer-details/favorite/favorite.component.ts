import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  Output
} from '@angular/core';
import { AuthUser } from '../../../auth/auth-user.model';
import { Beer } from '../../beer.model';
import { HttpErrorResponse } from '@angular/common/http';
import { faStar } from '@fortawesome/free-solid-svg-icons';
import { Subscription, map, take, tap } from 'rxjs';
import { FavoritesParams } from '../../../favorites/favorites-params';
import { FavoritesService } from '../../../favorites/favorites.service';
import { ModalService } from '../../../services/modal.service';
import {
  AlertService,
  AlertType
} from '../../../shared-components/alert/alert.service';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { ModalModel } from '../../../shared/modal-model';
import { ModalType } from '../../../shared/model-type';

@Component({
  selector: 'app-favorite',
  standalone: true,
  imports: [FontAwesomeModule],
  templateUrl: './favorite.component.html'
})
export class FavoriteComponent implements OnDestroy, OnChanges {
  @Input({ required: true }) beer!: Beer;
  @Input({ required: true }) user!: AuthUser | null;
  @Output() favoritesCountChanged = new EventEmitter<void>();

  private favoritesService: FavoritesService = inject(FavoritesService);
  private alertService: AlertService = inject(AlertService);
  private modalService: ModalService = inject(ModalService);

  loading = true;
  favorite = false;
  createFavoriteSubscription!: Subscription;
  deleteFavoriteSubscription!: Subscription;
  getUserFavoritesSubsciption!: Subscription;
  faStarr = faStar;

  ngOnChanges(): void {
    this.checkIfUserAlreadyAddedBeerToFavorites();
  }

  onToggleFavorite(): void {
    if (!this.user) {
      this.modalService.openModal(new ModalModel(ModalType.Login));
    } else if (!this.favorite) {
      this.loading = true;
      this.createFavoriteSubscription = this.favoritesService
        .createFavorite(this.beer.id)
        .subscribe({
          next: () => {
            this.favoritesCountChanged.emit();
            this.favorite = true;
            this.alertService.openAlert(
              AlertType.Success,
              'Successfully added to favorites'
            );
            this.loading = false;
          },
          error: error => {
            this.handleError(error);
          }
        });
    } else {
      this.loading = true;
      this.deleteFavoriteSubscription = this.favoritesService
        .deleteFavorite(this.beer.id)
        .subscribe({
          next: () => {
            this.favoritesCountChanged.emit();
            this.favorite = false;
            this.alertService.openAlert(
              AlertType.Success,
              'Successfully removed from favorites'
            );
            this.loading = false;
          },
          error: error => {
            this.handleError(error);
          }
        });
    }
  }

  private checkIfUserAlreadyAddedBeerToFavorites(): void {
    this.loading = true;
    if (!this.user) {
      this.favorite = false;
      this.loading = false;
    } else {
      this.getUserFavoritesSubsciption = this.favoritesService
        .getFavorites(
          new FavoritesParams({
            pageSize: 10,
            pageNumber: 1,
            beerId: this.beer.id,
            userId: this.user.id
          })
        )
        .pipe(
          take(1),
          map(beers => (beers.TotalCount > 0 ? true : false)),
          tap(favorite => {
            this.favorite = favorite;
            this.loading = false;
          })
        )
        .subscribe();
    }
  }

  private handleError(error: HttpErrorResponse): void {
    let errorMessage = null;

    if (error.error) {
      const firstKey = Object.keys(error.error?.errors)[0] ?? null;
      const firstValueArray = error.error?.errors[firstKey] as string[];
      errorMessage = firstValueArray[0];
    }

    if (!errorMessage) {
      this.alertService.openAlert(AlertType.Error, 'Something went wrong');
    } else {
      this.alertService.openAlert(AlertType.Error, errorMessage);
    }

    this.loading = false;
  }

  ngOnDestroy(): void {
    if (this.createFavoriteSubscription) {
      this.createFavoriteSubscription.unsubscribe();
    }
    if (this.deleteFavoriteSubscription) {
      this.deleteFavoriteSubscription.unsubscribe();
    }
    if (this.getUserFavoritesSubsciption) {
      this.getUserFavoritesSubsciption.unsubscribe();
    }
  }
}
