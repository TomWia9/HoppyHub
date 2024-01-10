import { Pagination } from './pagination';

export class PagedList<T> implements Pagination {
  constructor(
    public items: T[] = [],
    public currentPage: number,
    public totalPages: number,
    public totalCount: number,
    public hasPrevious: boolean,
    public hasNext: boolean
  ) {}
}
