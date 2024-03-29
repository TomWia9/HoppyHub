import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Opinion } from './opinion.model';
import { Observable, map } from 'rxjs';
import { OpinionsParams } from './opinions-params';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';

@Injectable({
  providedIn: 'root'
})
export class OpinionsService {
  private http: HttpClient = inject(HttpClient);

  getOpinions(opinionsParams: OpinionsParams): Observable<PagedList<Opinion>> {
    const params: HttpParams = opinionsParams.getHttpParams();

    return this.http
      .get<Opinion[]>(`${environment.opinionManagementApiUrl}/opinions`, {
        observe: 'response',
        params: params
      })
      .pipe(
        map((response: HttpResponse<Opinion[]>) => {
          const pagination = response.headers.get('X-Pagination');
          const paginationData: Pagination = JSON.parse(pagination!);
          return new PagedList<Opinion>(
            response.body as Opinion[],
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
