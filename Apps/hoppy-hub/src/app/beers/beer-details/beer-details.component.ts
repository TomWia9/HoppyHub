import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subject, map, switchMap, takeUntil, tap } from 'rxjs';
import { Beer } from '../beer.model';
import { BeersService } from '../beers.service';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { CommonModule } from '@angular/common';
import { BeerOpinionsComponent } from './beer-opinions/beer-opinions.component';
import { AuthService } from '../../auth/auth.service';
import { AuthUser } from '../../auth/auth-user.model';
import { FavoriteComponent } from './favorite/favorite.component';

@Component({
  selector: 'app-beer-details',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    RouterModule,
    CommonModule,
    BeerOpinionsComponent,
    FavoriteComponent
  ],
  templateUrl: './beer-details.component.html'
})
export class BeerDetailsComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private beersService: BeersService = inject(BeersService);
  private authService: AuthService = inject(AuthService);
  private destroy$ = new Subject<void>();

  beer!: Beer;
  error = '';
  loading = true;
  user: AuthUser | null = null;

  ngOnInit(): void {
    this.getBeer();
  }

  getBeer(): void {
    this.loading = true;

    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        switchMap(beerId =>
          this.beersService.getBeerById(beerId as string).pipe(
            tap({
              next: (beer: Beer) => {
                this.beer = beer;
                this.error = '';
                window.scrollTo({ top: 0, behavior: 'smooth' });
              },
              error: () => {
                this.error = 'An error occurred while loading the beer';
              }
            }),
            switchMap(() =>
              this.authService.user.pipe(
                tap(user => {
                  this.user = user;
                  this.loading = false;
                })
              )
            )
          )
        )
      )
      .subscribe();
  }

  refreshBeer(): void {
    this.beersService
      .getBeerById(this.beer.id)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: (beer: Beer) => {
            this.beer = beer;
            this.error = '';
            window.scrollTo({ top: 0, behavior: 'smooth' });
          },
          error: () => {
            this.error = 'An error occurred while loading the beer';
          }
        })
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
