import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class BeerStylesParams implements Params {
  public pageSize: number;
  public pageNumber: number;
  public sortBy?: string;
  public sortDirection?: number;
  public searchQuery?: string;
  public countryOfOrigin?: string;

  constructor(params: Partial<BeerStylesParams> = {}) {
    this.pageSize = params.pageSize ?? 10;
    this.pageNumber = params.pageNumber ?? 1;
    this.sortBy = params.sortBy;
    this.sortDirection = params.sortDirection;
    this.searchQuery = params.searchQuery;
    this.countryOfOrigin = params.countryOfOrigin;
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
    if (this.countryOfOrigin && this.countryOfOrigin.trim() !== '') {
      params = params.append('countryOfOrigin', this.countryOfOrigin);
    }

    return params;
  }

  static sortOptions = [
    { label: 'Name (A to Z)', value: 'Name', direction: 0 },
    { label: 'Name (Z to A)', value: 'Name', direction: 1 },
    {
      label: 'Country Of Origin (A to Z)',
      value: 'CountryOfOrigin',
      direction: 0
    },
    {
      label: 'Country Of Origin (Z to A)',
      value: 'CountryOfOrigin',
      direction: 1
    }
  ];
}
