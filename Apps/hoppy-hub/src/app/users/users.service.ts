import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { User } from './user.model';

const Users_Api = 'http://localhost:5049/api/users/';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private http: HttpClient = inject(HttpClient);

  opinionsChanged = new Subject<User>();

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${Users_Api}${id}`);
  }
}
