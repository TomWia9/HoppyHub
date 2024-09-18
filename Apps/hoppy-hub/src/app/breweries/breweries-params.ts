import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class BreweriesParams implements Params {
  public pageSize: number;
  public pageNumber: number;
  public sortBy?: string;
  public sortDirection?: number;
  public searchQuery?: string;
  public name?: string;
  public country?: string;
  public state?: string;
  public city?: string;
  public minFoundationYear?: number;
  public maxFoundationYear?: number;

  constructor(params: Partial<BreweriesParams> = {}) {
    this.pageSize = params.pageSize ?? 10;
    this.pageNumber = params.pageNumber ?? 1;
    this.sortBy = params.sortBy;
    this.sortDirection = params.sortDirection;
    this.searchQuery = params.searchQuery;
    this.name = params.name;
    this.country = params.country;
    this.state = params.state;
    this.city = params.city;
    this.minFoundationYear = params.minFoundationYear;
    this.maxFoundationYear = params.maxFoundationYear;
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
    if (this.name && this.name.trim() !== '') {
      params = params.append('name', this.name);
    }
    if (this.country && this.country.trim() !== '') {
      params = params.append('country', this.country);
    }
    if (this.state && this.state.trim() !== '') {
      params = params.append('state', this.state);
    }
    if (this.city && this.city.trim() !== '') {
      params = params.append('city', this.city);
    }
    if (this.minFoundationYear) {
      params = params.append('minFoundationYear', this.minFoundationYear);
    }
    if (this.maxFoundationYear) {
      params = params.append('maxFoundationYear', this.maxFoundationYear);
    }

    return params;
  }
}
