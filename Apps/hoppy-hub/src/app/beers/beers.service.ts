import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Beer } from './beer.model';

const Beers_Api = 'http://localhost:5206/api/beers/';

@Injectable({
  providedIn: 'root'
})
export class BeersService {
  private http: HttpClient = inject(HttpClient);

  getBeerById(id: string): Observable<Beer> {
    return this.http.get<Beer>(`${Beers_Api}${id}`);
  }
}
