import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class FavoritesParams implements Params {
  public pageSize: number;
  public pageNumber: number;
  public sortBy?: string;
  public sortDirection?: number;
  public searchQuery?: string;
  public beerId?: string;
  public userId?: string;

  constructor(params: Partial<FavoritesParams> = {}) {
    this.pageSize = params.pageSize ?? 10;
    this.pageNumber = params.pageNumber ?? 1;
    this.sortBy = params.sortBy;
    this.sortDirection = params.sortDirection;
    this.searchQuery = params.searchQuery;
    this.beerId = params.beerId;
    this.userId = params.userId;
  }

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
    if (this.beerId && this.beerId.trim() != '') {
      params = params.append('beerId', this.beerId);
    }
    if (this.userId && this.userId.trim() != '') {
      params = params.append('userId', this.userId);
    }

    return params;
  }
}
