import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { Beer } from './beer.model';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { BeersParams } from './beers-params';
import { UpsertBeerCommand } from './upsert-beer-command.model';

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

  CreateBeer(upsertBeerCommand: UpsertBeerCommand): Observable<Beer> {
    const formData = this.buildFormData(upsertBeerCommand);

    return this.http.post<Beer>(
      `${environment.beerManagementApiUrl}/beers`,
      formData
    );
  }

  UpdateBeer(
    beerId: string,
    upsertBeerCommand: UpsertBeerCommand
  ): Observable<void> {
    const formData = this.buildFormData(upsertBeerCommand);

    return this.http.put<void>(
      `${environment.beerManagementApiUrl}/beers/${beerId}`,
      formData
    );
  }

  DeleteBeer(beerId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.beerManagementApiUrl}/beers/${beerId}`
    );
  }

  private buildFormData(upsertBeerCommand: UpsertBeerCommand): FormData {
    const formData = new FormData();
    if (upsertBeerCommand.id) {
      formData.append('Id', upsertBeerCommand.id);
    }
    formData.append('Name', upsertBeerCommand.name);
    formData.append('BreweryId', upsertBeerCommand.breweryId);
    formData.append(
      'AlcoholByVolume',
      upsertBeerCommand.alcoholByVolume.toString()
    );
    formData.append('Description', upsertBeerCommand.description);
    formData.append('Composition', upsertBeerCommand.composition);
    formData.append('Blg', upsertBeerCommand.blg.toString());
    formData.append('BeerStyleId', upsertBeerCommand.beerStyleId);
    formData.append('Ibu', upsertBeerCommand.ibu.toString());
    formData.append(
      'releaseDate',
      upsertBeerCommand.releaseDate.toDateString()
    );

    return formData;
  }
}
