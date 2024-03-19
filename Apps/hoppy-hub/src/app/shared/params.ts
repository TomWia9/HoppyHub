import { HttpParams } from '@angular/common/http';

export interface Params {
  pageSize: number;
  pageNumber: number;
  sortBy?: string;
  sortDirection?: number;
  searchQuery?: string;

  getHttpParams(): HttpParams;
}
