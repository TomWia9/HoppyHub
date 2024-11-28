import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from './user.model';
import { environment } from '../../environments/environment';
import { UpdateUserPasswordCommand } from './commands/update-user-password-command.model';
import { UpdateUsernameCommand } from './commands/update-username-command.model';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private http: HttpClient = inject(HttpClient);

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(
      `${environment.userManagementApiUrl}/users/${id}`
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

  DeleteOpinion(userId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.userManagementApiUrl}/users/${userId}`
    );
  }
}
