import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
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

  paramsChanged = new BehaviorSubject<BeersParams>(
    new BeersParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'releaseDate',
      sortDirection: 1
    })
  );

  getBeerById(id: string): Observable<Beer> {
    return this.http.get<Beer>(
      `${environment.beerManagementApiUrl}/beers/${id}`
    );
  }

  getBeers(beersParams: BeersParams): Observable<PagedList<Beer>> {
    console.log(beersParams);

    const params: HttpParams = beersParams.getHttpParams();
    console.log(params);

    return this.http
      .get<Beer[]>(`${environment.beerManagementApiUrl}/beers`, {
        observe: 'response',
        params: params
      })
      .pipe(
        map((response: HttpResponse<Beer[]>) => {
          const pagination = response.headers.get('X-Pagination');
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
