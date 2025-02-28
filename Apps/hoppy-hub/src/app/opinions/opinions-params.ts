import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class OpinionsParams implements Params {
  public pageSize: number;
  public pageNumber: number;
  public sortBy?: string;
  public sortDirection?: number;
  public searchQuery?: string;
  public minRating?: number;
  public maxRating?: number;
  public from?: string;
  public to?: string;
  public hasImage?: boolean;
  public hasComment?: boolean;
  public beerId?: string;
  public userId?: string;

  constructor(params: Partial<OpinionsParams> = {}) {
    this.pageSize = params.pageSize ?? 10;
    this.pageNumber = params.pageNumber ?? 1;
    this.sortBy = params.sortBy;
    this.sortDirection = params.sortDirection;
    this.searchQuery = params.searchQuery;
    this.minRating = params.minRating;
    this.maxRating = params.maxRating;
    this.from = params.from;
    this.to = params.to;
    this.hasImage = params.hasImage;
    this.hasComment = params.hasComment;
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
    if (this.minRating) {
      params = params.append('minRating', this.minRating);
    }
    if (this.maxRating) {
      params = params.append('maxRating', this.maxRating);
    }
    if (this.from) {
      params = params.append('from', this.from);
    }
    if (this.to) {
      params = params.append('to', this.to);
    }
    if (this.hasImage != null) {
      params = params.append('hasImage', this.hasImage);
    }
    if (this.hasComment != null) {
      params = params.append('hasComment', this.hasComment);
    }
    if (this.beerId && this.beerId.trim() != '') {
      params = params.append('beerId', this.beerId);
    }
    if (this.userId && this.userId.trim() != '') {
      params = params.append('userId', this.userId);
    }

    return params;
  }

  static sortOptions = [
    {
      label: 'Created (New to Old)',
      value: 'Created',
      direction: 1
    },
    {
      label: 'Created (Old to New)',
      value: 'Created',
      direction: 0
    },
    {
      label: 'Last Modified (New to Old)',
      value: 'LastModified',
      direction: 1
    },
    {
      label: 'Last Modified (Old to New)',
      value: 'LastModified',
      direction: 0
    },
    { label: 'Rating (High to Low)', value: 'Rating', direction: 1 },
    { label: 'Rating (Low to High)', value: 'Rating', direction: 0 }
  ];
}
