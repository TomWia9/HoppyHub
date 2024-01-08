import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResult } from './auth-result.model';
import { User } from './user.model';
import { jwtDecode } from 'jwt-decode';
import { TokenClaims } from './token-claims.model';

const AUTH_API = 'http://localhost:5049/api/identity/';
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  user = new BehaviorSubject<User | null>(null);

  login(email: string, password: string): Observable<AuthResult> {
    return this.http
      .post<AuthResult>(
        AUTH_API + 'login',
        {
          email,
          password
        },
        httpOptions
      )
      .pipe(
        tap(result => {
          if (result.token) {
            this.handleAuthentication(result.token);
          }
        })
      );
  }

  register(
    email: string,
    username: string,
    password: string
  ): Observable<AuthResult> {
    return this.http
      .post<AuthResult>(AUTH_API + 'register', {
        email,
        username,
        password
      })
      .pipe(
        tap(result => {
          if (result.token) {
            this.handleAuthentication(result.token);
          }
        })
      );
  }

  handleAuthentication(token: string) {
    const decodedToken: TokenClaims = jwtDecode(token);
    const expirationDate = new Date(+decodedToken.exp * 1000);
    const user = new User(
      decodedToken.sub,
      decodedToken.email,
      decodedToken.role,
      token,
      expirationDate
    );
    this.user.next(user);

    localStorage.setItem('userData', JSON.stringify(user));
  }
}
