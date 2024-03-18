import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Brewery } from './brewery.model';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { BreweriesParams } from './breweries-params';

@Injectable({
  providedIn: 'root'
})
export class BreweriesService {
  private http: HttpClient = inject(HttpClient);

  getBreweries(
    breweriesParams: BreweriesParams
  ): Observable<PagedList<Brewery>> {
    const params: HttpParams = breweriesParams.getHttpParams();

    return this.http
      .get<Brewery[]>(`${environment.beerManagementApiUrl}/breweries`, {
        observe: 'response',
        params: params
      })
      .pipe(
        map((response: HttpResponse<Brewery[]>) => {
          const pagination = response.headers.get('X-Pagination');
          const paginationData: Pagination = JSON.parse(pagination!);
          return new PagedList<Brewery>(
            response.body as Brewery[],
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
