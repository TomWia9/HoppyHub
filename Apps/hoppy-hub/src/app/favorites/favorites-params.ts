import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class FavoritesParams implements Params {
  constructor(
    public pageSize: number,
    public pageNumber: number,
    public sortBy?: string,
    public sortDirection?: number,
    public searchQuery?: string,
    public userId?: string
  ) {}

  getHttpParams(): HttpParams {
    let params = new HttpParams();
    params = params.append('pageNumber', this.pageNumber);
    params = params.append('pageSize', this.pageSize);

    if (this.sortBy && this.sortBy.trim() !== '') {
      params = params.append('sortBy', this.sortBy);
    }
    if (this.sortDirection) {
      params = params.append('sortDirection', this.sortDirection);
    }
    if (this.searchQuery && this.searchQuery.trim() !== '') {
      params = params.append('searchQuery', this.searchQuery);
    }
    if (this.userId && this.userId.trim() != '') {
      params = params.append('userId', this.userId);
    }

    return params;
  }
}
