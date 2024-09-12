import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Opinion } from './opinion.model';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { OpinionsParams } from './opinions-params';
import { environment } from '../../environments/environment';
import { PagedList } from '../shared/paged-list';
import { Pagination } from '../shared/pagination';
import { UpsertOpinionCommand } from './upsert-opinion-command.model';

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
    upsertOpinionCommand: UpsertOpinionCommand
  ): Observable<Opinion> {
    const formData = this.buildFormData(upsertOpinionCommand);

    return this.http.post<Opinion>(
      `${environment.opinionManagementApiUrl}/opinions`,
      formData
    );
  }

  UpdateOpinion(
    opinionId: string,
    upsertOpinionCommand: UpsertOpinionCommand
  ): Observable<void> {
    const formData = this.buildFormData(upsertOpinionCommand);

    return this.http.put<void>(
      `${environment.opinionManagementApiUrl}/opinions/${opinionId}`,
      formData
    );
  }

  private buildFormData(upsertOpinionCommand: UpsertOpinionCommand): FormData {
    const formData = new FormData();
    if (upsertOpinionCommand.id) {
      formData.append('Id', upsertOpinionCommand.id);
    }
    formData.append('BeerId', upsertOpinionCommand.beerId);
    formData.append('Rating', upsertOpinionCommand.rating.toString());
    formData.append('Comment', upsertOpinionCommand.comment);

    if (upsertOpinionCommand.image) {
      formData.append('Image', upsertOpinionCommand.image);
    }
    return formData;
  }
}
