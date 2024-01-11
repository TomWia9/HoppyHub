import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Beer } from './beer.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BeersService {
  private http: HttpClient = inject(HttpClient);

  getBeerById(id: string): Observable<Beer> {
    return this.http.get<Beer>(
      `${environment.beerManagementApiUrl}/beers/${id}`
    );
  }
}
