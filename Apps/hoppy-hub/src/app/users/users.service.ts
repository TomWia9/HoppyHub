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
    const formData = new FormData();
    if (updateUsernameCommand.userId) {
      formData.append('UserId', updateUsernameCommand.userId);
    }
    if (updateUsernameCommand.username) {
      formData.append('Username', updateUsernameCommand.username);
    }

    return this.http.put<void>(
      `${environment.userManagementApiUrl}/users/${userId}`,
      formData
    );
  }

  UpdateUserPassword(
    userId: string,
    updateUserPasswordCommand: UpdateUserPasswordCommand
  ): Observable<void> {
    const formData = new FormData();
    if (updateUserPasswordCommand.userId) {
      formData.append('UserId', updateUserPasswordCommand.userId);
    }
    if (updateUserPasswordCommand.currentPassword) {
      formData.append(
        'CurrentPassword',
        updateUserPasswordCommand.currentPassword
      );
    }
    if (updateUserPasswordCommand.newPassword) {
      formData.append('NewPassword', updateUserPasswordCommand.newPassword);
    }

    return this.http.put<void>(
      `${environment.userManagementApiUrl}/users/${userId}/changePassword`,
      formData
    );
  }

  DeleteOpinion(userId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.userManagementApiUrl}/users/${userId}`
    );
  }
}
