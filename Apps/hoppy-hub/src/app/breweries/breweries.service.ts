import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Brewery } from './brewery.model';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { BreweriesParams } from './breweries-params';
import { UpsertBreweryCommand } from './upsert-brewery-command.model';

@Injectable({
  providedIn: 'root'
})
export class BreweriesService {
  private http: HttpClient = inject(HttpClient);

  paramsChanged = new BehaviorSubject<BreweriesParams>(
    new BreweriesParams({
      pageSize: 25,
      pageNumber: 1,
      sortBy: 'Name',
      sortDirection: 1
    })
  );

  getBreweryById(id: string): Observable<Brewery> {
    return this.http.get<Brewery>(
      `${environment.beerManagementApiUrl}/breweries/${id}`
    );
  }

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

  createBrewery(
    upsertBreweryCommand: UpsertBreweryCommand
  ): Observable<Brewery> {
    return this.http.post<Brewery>(
      `${environment.beerManagementApiUrl}/breweries`,
      upsertBreweryCommand
    );
  }

  updateBrewery(
    breweryId: string,
    upsertBreweryCommand: UpsertBreweryCommand
  ): Observable<void> {
    return this.http.put<void>(
      `${environment.beerManagementApiUrl}/breweries/${breweryId}`,
      upsertBreweryCommand
    );
  }

  deleteBrewery(breweryId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.beerManagementApiUrl}/breweries/${breweryId}`
    );
  }
}
