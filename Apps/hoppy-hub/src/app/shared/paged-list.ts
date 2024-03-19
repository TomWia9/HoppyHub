import { Pagination } from './pagination';

export class PagedList<T> implements Pagination {
  constructor(
    public items: T[] = [],
    public CurrentPage: number,
    public TotalPages: number,
    public TotalCount: number,
    public HasPrevious: boolean,
    public HasNext: boolean
  ) {}
}
