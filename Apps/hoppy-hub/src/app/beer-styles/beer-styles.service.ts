import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { BeerStylesParams } from './beer-styles-params';
import { BeerStyle } from './beer-style.model';
import { UpsertBeerStyleCommand } from './upsert-beer-style-command.model';

@Injectable({
  providedIn: 'root'
})
export class BeerStylesService {
  private http: HttpClient = inject(HttpClient);

  paramsChanged = new BehaviorSubject<BeerStylesParams>(
    new BeerStylesParams({
      pageSize: 15,
      pageNumber: 1
    })
  );

  getBeerStyles(
    beerStylesParams: BeerStylesParams
  ): Observable<PagedList<BeerStyle>> {
    const params: HttpParams = beerStylesParams.getHttpParams();

    return this.http
      .get<BeerStyle[]>(`${environment.beerManagementApiUrl}/beerstyles`, {
        observe: 'response',
        params: params
      })
      .pipe(
        map((response: HttpResponse<BeerStyle[]>) => {
          const pagination = response.headers.get('X-Pagination');
          const paginationData: Pagination = JSON.parse(pagination!);
          return new PagedList<BeerStyle>(
            response.body as BeerStyle[],
            paginationData.CurrentPage,
            paginationData.TotalPages,
            paginationData.TotalCount,
            paginationData.HasPrevious,
            paginationData.HasNext
          );
        })
      );
  }

  getBeerStyleById(id: string): Observable<BeerStyle> {
    return this.http.get<BeerStyle>(
      `${environment.beerManagementApiUrl}/beerstyles/${id}`
    );
  }

  createBeerStyle(
    upsertBeerStyleCommand: UpsertBeerStyleCommand
  ): Observable<BeerStyle> {
    return this.http.post<BeerStyle>(
      `${environment.beerManagementApiUrl}/beerstyles`,
      upsertBeerStyleCommand
    );
  }

  updateBeerStyle(
    beerStyleId: string,
    upsertBeerStyleCommand: UpsertBeerStyleCommand
  ): Observable<void> {
    return this.http.put<void>(
      `${environment.beerManagementApiUrl}/beerstyles/${beerStyleId}`,
      upsertBeerStyleCommand
    );
  }

  deleteBeerStyle(beerStyleId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.beerManagementApiUrl}/beerstyles/${beerStyleId}`
    );
  }
}
