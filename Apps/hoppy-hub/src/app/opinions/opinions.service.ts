import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Opinion } from './opinion.model';
import { Subject, Observable } from 'rxjs';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { OpinionsParams } from './opinions-params';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OpinionsService {
  private http: HttpClient = inject(HttpClient);

  opinionsChanged = new Subject<PagedList<Opinion>>();
  errorCatched = new Subject<string>();
  loading = new Subject<boolean>();

  getOpinions(opinionsParams: OpinionsParams): void {
    this.loading.next(true);
    const params: HttpParams = opinionsParams.getHttpParams();

    this.fetchOpinions(params).subscribe({
      next: (response: HttpResponse<Opinion[]>) => {
        const pagination = response.headers.get('X-Pagination');
        const paginationData: Pagination = JSON.parse(pagination!);
        const opinions = new PagedList<Opinion>(
          response.body!,
          paginationData.CurrentPage,
          paginationData.TotalPages,
          paginationData.TotalCount,
          paginationData.HasPrevious,
          paginationData.HasNext
        );

        this.opinionsChanged.next(opinions);
        this.loading.next(false);
      },
      error: () => {
        this.errorCatched.next('An error occurred while loading the opinions');

        this.loading.next(false);
      }
    });
  }

  private fetchOpinions(
    params: HttpParams
  ): Observable<HttpResponse<Opinion[]>> {
    return this.http.get<Opinion[]>(
      `${environment.opinionManagementApiUrl}/opinions`,
      {
        observe: 'response',
        params: params
      }
    );
  }
}
