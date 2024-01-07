import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  AUTH_API: string = 'http://localhost:5001/api/identity/';

  private http = inject(HttpClient);
  private router = inject(Router);
}
