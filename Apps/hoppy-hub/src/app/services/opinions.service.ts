import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Opinion } from '../opinions/opinion.model';
import { Subject, Observable } from 'rxjs';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { OpinionsParams } from '../opinions/opinions-params';

const Opinions_Api = 'http://localhost:5110/api/opinions/';

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
          paginationData.currentPage,
          paginationData.totalPages,
          paginationData.totalCount,
          paginationData.hasPrevious,
          paginationData.hasNext
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
    return this.http.get<Opinion[]>(Opinions_Api, {
      observe: 'response',
      params: params
    });
  }
}
