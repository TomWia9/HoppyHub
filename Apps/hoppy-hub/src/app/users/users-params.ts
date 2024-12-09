import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class UsersParams implements Params {
  public pageSize: number;
  public pageNumber: number;
  public sortBy?: string;
  public sortDirection?: number;
  public searchQuery?: string;
  public role?: string;

  constructor(params: Partial<UsersParams> = {}) {
    this.pageSize = params.pageSize ?? 10;
    this.pageNumber = params.pageNumber ?? 1;
    this.sortBy = params.sortBy;
    this.sortDirection = params.sortDirection;
    this.searchQuery = params.searchQuery;
    this.role = params.role;
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
    if (this.role && this.role.trim() !== '') {
      params = params.append('role', this.role);
    }

    return params;
  }

  static sortOptions = [
    {
      label: 'Email (A to Z)',
      value: 'Email',
      direction: 0
    },
    {
      label: 'Email (Z to A)',
      value: 'Email',
      direction: 1
    },
    { label: 'Username (A to Z)', value: 'Username', direction: 0 },
    { label: 'Username (Z to A)', value: 'Username', direction: 1 },
    {
      label: 'Created (New to Old)',
      value: 'Created',
      direction: 1
    },
    {
      label: 'Created (Old to New)',
      value: 'Created',
      direction: 0
    }
  ];
}
