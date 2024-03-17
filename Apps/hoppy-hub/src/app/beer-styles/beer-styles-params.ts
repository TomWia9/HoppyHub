import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class BeerStylesParams implements Params {
  constructor(
    public pageSize: number,
    public pageNumber: number,
    public sortBy?: string,
    public sortDirection?: number,
    public searchQuery?: string,
    public countryOfOrigin?: string
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
    if (this.countryOfOrigin && this.countryOfOrigin.trim() !== '') {
      params = params.append('countryOfOrigin', this.countryOfOrigin);
    }

    return params;
  }
}
