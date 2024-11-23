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
    new OpinionsParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'created',
      sortDirection: 1
    })
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

  DeleteOpinion(opinionId: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.opinionManagementApiUrl}/opinions/${opinionId}`
    );
  }

  private buildFormData(upsertOpinionCommand: UpsertOpinionCommand): FormData {
    const formData = new FormData();
    if (upsertOpinionCommand.id) {
      formData.append('Id', upsertOpinionCommand.id);
    }
    if (upsertOpinionCommand.image) {
      formData.append('Image', upsertOpinionCommand.image);
    }
    if (upsertOpinionCommand.deleteImage) {
      formData.append(
        'DeleteImage',
        upsertOpinionCommand.deleteImage.toString()
      );
    }
    formData.append('BeerId', upsertOpinionCommand.beerId);
    formData.append('Rating', upsertOpinionCommand.rating.toString());
    formData.append('Comment', upsertOpinionCommand.comment);

    return formData;
  }
}
