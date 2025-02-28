import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UsersService } from '../../../users/users.service';
import { Subject, takeUntil, tap } from 'rxjs';
import { DataHelper } from '../../../shared/data-helper';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { User } from '../../../users/user.model';
import { UsersParams } from '../../../users/users-params';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { UsersListFiltersComponent } from './users-list-filters/users-list-filters.component';
import { ErrorMessageComponent } from '../../../shared-components/error-message/error-message.component';
import { RouterModule } from '@angular/router';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    UsersListFiltersComponent,
    ErrorMessageComponent,
    RouterModule,
    PaginationComponent,
    CommonModule
  ],
  templateUrl: './users-list.component.html'
})
export class UsersListComponent
  extends DataHelper
  implements OnInit, OnDestroy
{
  private usersService: UsersService = inject(UsersService);
  private destroy$ = new Subject<void>();

  usersParams = new UsersParams({
    pageSize: 15,
    pageNumber: 1,
    sortBy: 'Created',
    sortDirection: 1
  });
  users: PagedList<User> | undefined;
  paginationData!: Pagination;
  error = '';
  loading = true;

  ngOnInit(): void {
    this.usersService.paramsChanged
      .pipe(
        takeUntil(this.destroy$),
        tap((params: UsersParams) => {
          this.usersParams = params;
          this.getUsers();
        })
      )
      .subscribe();
  }

  private getUsers(): void {
    this.usersService
      .getUsers(this.usersParams)
      .pipe(
        takeUntil(this.destroy$),
        tap({
          next: (users: PagedList<User>) => {
            this.users = users;
            this.paginationData = this.getPaginationData(users);
            this.error = '';
          },
          error: error => {
            this.error = 'An error occurred while loading the users';

            if (error.error && error.error.errors) {
              const errorMessage = this.getValidationErrorMessage(
                error.error.errors
              );
              this.error += errorMessage;
            }
          },
          complete: () => {
            this.loading = false;
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
