import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, Subject, map } from 'rxjs';
import { Beer } from './beer.model';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { BeersParams } from './beers-params';

@Injectable({
  providedIn: 'root'
})
export class BeersService {
  private http: HttpClient = inject(HttpClient);

  paramsChanged = new Subject<BeersParams>();

  getBeerById(id: string): Observable<Beer> {
    return this.http.get<Beer>(
      `${environment.beerManagementApiUrl}/beers/${id}`
    );
  }

  getBeers(beersParams: BeersParams): Observable<PagedList<Beer>> {
    const params: HttpParams = beersParams.getHttpParams();

    return this.http
      .get<Beer[]>(`${environment.beerManagementApiUrl}/beers`, {
        observe: 'response',
        params: params
      })
      .pipe(
        map((response: HttpResponse<Beer[]>) => {
          console.log('RESPONSE ');
          console.log(response);

          const pagination = response.headers.get('X-Pagination');
          console.log('BeersService - map - pagination: ');
          console.log(pagination);

          const paginationData: Pagination = JSON.parse(pagination!);

          return new PagedList<Beer>(
            response.body as Beer[],
            paginationData.CurrentPage,
            paginationData.TotalPages,
            paginationData.TotalCount,
            paginationData.HasPrevious,
            paginationData.HasNext
          );
        })
      );
  }
}
