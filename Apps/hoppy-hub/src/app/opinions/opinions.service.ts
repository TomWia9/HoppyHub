import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Opinion } from './opinion.model';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { OpinionsParams } from './opinions-params';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { CreateOpinionCommand } from './create-opinion-command.model';

@Injectable({
  providedIn: 'root'
})
export class OpinionsService {
  private http: HttpClient = inject(HttpClient);

  paramsChanged = new BehaviorSubject<OpinionsParams>(
    new OpinionsParams(10, 1, 'created', 1)
  );

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

  CreateOpinion(
    createOpinionCommand: CreateOpinionCommand
  ): Observable<Opinion> {
    const formData = this.buildFormData(createOpinionCommand);

    return this.http.post<Opinion>(
      `${environment.opinionManagementApiUrl}/opinions`,
      formData
    );
  }

  private buildFormData(createOpinionCommand: CreateOpinionCommand): FormData {
    const formData = new FormData();
    formData.append('BeerId', createOpinionCommand.beerId);
    formData.append('Rating', createOpinionCommand.rating.toString());
    formData.append('Comment', createOpinionCommand.comment);

    if (createOpinionCommand.image) {
      formData.append('Image', createOpinionCommand.image);
    }
    return formData;
  }
}
