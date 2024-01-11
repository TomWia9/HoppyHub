import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { User } from './user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private http: HttpClient = inject(HttpClient);

  opinionsChanged = new Subject<User>();

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(
      `${environment.userManagementApiUrl}/users/${id}`
    );
  }
}
