import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class OpinionsParams implements Params {
  constructor(
    public pageSize: number,
    public pageNumber: number,
    public sortBy?: string,
    public sortDirection?: number,
    public searchQuery?: string,
    public minRating?: number,
    public maxRating?: number,
    public haveImage?: boolean,
    public beerId?: string,
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
    if (this.minRating) {
      params = params.append('minRating', this.minRating);
    }
    if (this.maxRating) {
      params = params.append('maxRating', this.maxRating);
    }
    if (this.haveImage) {
      params = params.append('haveImage', this.haveImage);
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
