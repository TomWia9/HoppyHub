import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { Beer } from './beer.model';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { BeersParams } from './beers-params';
import { UpsertBeerCommand } from './upsert-beer-command.model';
import { UpsertBeerImageCommand } from './upsert-beer-image-command.model';

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
    const params: HttpParams = beersParams.getHttpParams();

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

  createBeer(upsertBeerCommand: UpsertBeerCommand): Observable<Beer> {
    return this.http.post<Beer>(
      `${environment.beerManagementApiUrl}/beers`,
      upsertBeerCommand
    );
  }

  updateBeer(
    beerId: string,
    upsertBeerCommand: UpsertBeerCommand
  ): Observable<void> {
    return this.http.put<void>(
      `${environment.beerManagementApiUrl}/beers/${beerId}`,
      upsertBeerCommand
    );
  }

  deleteBeer(beerId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.beerManagementApiUrl}/beers/${beerId}`
    );
  }

  upsertBeerImage(
    beerId: string,
    upsertBeerImageCommand: UpsertBeerImageCommand
  ): Observable<string> {
    const formData = new FormData();
    formData.append('BeerId', upsertBeerImageCommand.beerId);
    if (upsertBeerImageCommand.image) {
      formData.append('Image', upsertBeerImageCommand.image);
    }

    return this.http.post<string>(
      `${environment.beerManagementApiUrl}/beers/${beerId}/upsertImage`,
      formData,
      { responseType: 'text' as 'json' }
    );
  }

  deleteBeerImage(beerId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.beerManagementApiUrl}/beers/${beerId}/deleteImage`
    );
  }
}
