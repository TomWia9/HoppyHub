import { inject, Injectable } from '@angular/core';
import { Beer } from '../beers/beer.model';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { FavoritesParams } from './favorites-params';

@Injectable({
  providedIn: 'root'
})
export class FavoritesServiceService {
  private http: HttpClient = inject(HttpClient);

  paramsChanged = new BehaviorSubject<FavoritesParams>(
    new FavoritesParams(10, 1)
  );

  getFavorites(favoritesParams: FavoritesParams): Observable<PagedList<Beer>> {
    const params: HttpParams = favoritesParams.getHttpParams();

    return this.http
      .get<Beer[]>(`${environment.favoriteManagementApiUrl}/favorites`, {
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

  CreateFavorite(beerId: string): Observable<void> {
    return this.http.post<void>(
      `${environment.favoriteManagementApiUrl}/favorites/${beerId}`,
      beerId
    );
  }

  DeleteFavorite(beerId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.opinionManagementApiUrl}/favorites/${beerId}`
    );
  }
}
