import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { User } from './user.model';
import { environment } from '../../environments/environment';
import { UpdateUserPasswordCommand } from './commands/update-user-password-command.model';
import { UpdateUsernameCommand } from './commands/update-username-command.model';
import { DeleteUserCommand } from './commands/delete-user-command.model';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { UsersParams } from './users-params';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private http: HttpClient = inject(HttpClient);

  paramsChanged = new BehaviorSubject<UsersParams>(
    new UsersParams({
      pageSize: 15,
      pageNumber: 1,
      sortBy: 'Created',
      sortDirection: 1
    })
  );

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(
      `${environment.userManagementApiUrl}/users/${id}`
    );
  }

  getUsers(usersParams: UsersParams): Observable<PagedList<User>> {
    const params: HttpParams = usersParams.getHttpParams();

    return this.http
      .get<User[]>(`${environment.userManagementApiUrl}/users`, {
        observe: 'response',
        params: params
      })
      .pipe(
        map((response: HttpResponse<User[]>) => {
          const pagination = response.headers.get('X-Pagination');
          const paginationData: Pagination = JSON.parse(pagination!);

          return new PagedList<User>(
            response.body as User[],
            paginationData.CurrentPage,
            paginationData.TotalPages,
            paginationData.TotalCount,
            paginationData.HasPrevious,
            paginationData.HasNext
          );
        })
      );
  }

  UpdateUsername(
    userId: string,
    updateUsernameCommand: UpdateUsernameCommand
  ): Observable<void> {
    return this.http.put<void>(
      `${environment.userManagementApiUrl}/users/${userId}`,
      updateUsernameCommand
    );
  }

  UpdateUserPassword(
    userId: string,
    updateUserPasswordCommand: UpdateUserPasswordCommand
  ): Observable<void> {
    return this.http.put<void>(
      `${environment.userManagementApiUrl}/users/${userId}/changePassword`,
      updateUserPasswordCommand
    );
  }

  DeleteAccount(
    userId: string,
    deleteUserCommand: DeleteUserCommand
  ): Observable<void> {
    return this.http.delete<void>(
      `${environment.userManagementApiUrl}/users/${userId}`,
      { body: deleteUserCommand }
    );
  }
}
