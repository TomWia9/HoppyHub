import { PagedList } from './paged-list';
import { Pagination } from './pagination';

export abstract class DataHelper {
  protected getPaginationData<T>(pagedList: PagedList<T>): Pagination {
    if (pagedList) {
      return {
        CurrentPage: pagedList.CurrentPage,
        HasNext: pagedList.HasNext,
        HasPrevious: pagedList.HasPrevious,
        TotalPages: pagedList.TotalPages,
        TotalCount: pagedList.TotalCount
      };
    }

    return {
      CurrentPage: 0,
      HasNext: false,
      HasPrevious: false,
      TotalPages: 0,
      TotalCount: 0
    };
  }

  protected getValidationErrorMessage(
    array: { [key: string]: string }[]
  ): string {
    if (array.length === 0) {
      return '';
    }

    const firstObject = Object.values(array)[0];
    const errorMessage = Object.values(firstObject)[0];

    if (!errorMessage) {
      return '';
    }

    return ': ' + errorMessage;
  }
}
