import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserInfoComponent } from './user-info/user-info.component';
import { map, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { User } from '../user.model';
import { UsersService } from '../users.service';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { UserOpinionsComponent } from './user-opinions/user-opinions.component';
import { UserFavoritesComponent } from './user-favorites/user-favorites.component';
import { AuthService } from '../../auth/auth.service';
import { Roles } from '../../auth/roles';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [
    UserInfoComponent,
    LoadingSpinnerComponent,
    ErrorMessageComponent,
    UserOpinionsComponent,
    UserFavoritesComponent
  ],
  templateUrl: './user-details.component.html'
})
export class UserDetailsComponent implements OnInit, OnDestroy {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private usersService: UsersService = inject(UsersService);
  private authService: AuthService = inject(AuthService);
  private destroy$ = new Subject<void>();

  user!: User;
  editAccess: boolean = false;
  error = '';
  loading = true;

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        map(params => params.get('id')),
        switchMap(userId =>
          this.usersService.getUserById(userId as string).pipe(
            tap({
              next: (user: User) => {
                this.user = user;
                this.error = '';
              },
              error: () => {
                this.error = 'An error occurred while loading the user';
              }
            }),
            switchMap(() =>
              this.authService.user.pipe(
                tap(user => {
                  this.editAccess =
                    this.user.id === user?.id ||
                    user?.role === Roles.Administrator;
                  this.loading = false;
                })
              )
            )
          )
        )
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
