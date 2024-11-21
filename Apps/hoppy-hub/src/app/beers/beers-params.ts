import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class BeersParams implements Params {
  public pageSize: number;
  public pageNumber: number;
  public sortBy?: string;
  public sortDirection?: number;
  public searchQuery?: string;
  public name?: string;
  public breweryId?: string;
  public beerStyleId?: string;
  public minAlcoholByVolume?: number;
  public maxAlcoholByVolume?: number;
  public minExtract?: number;
  public maxExtract?: number;
  public minIbu?: number;
  public maxIbu?: number;
  public minReleaseDate?: string;
  public maxReleaseDate?: string;
  public minRating?: number;
  public maxRating?: number;
  public minFavoritesCount?: number;
  public maxFavoritesCount?: number;
  public minOpinionsCount?: number;
  public maxOpinionsCount?: number;

  constructor(params: Partial<BeersParams> = {}) {
    this.pageSize = params.pageSize ?? 10;
    this.pageNumber = params.pageNumber ?? 1;
    this.sortBy = params.sortBy;
    this.sortDirection = params.sortDirection;
    this.searchQuery = params.searchQuery;
    this.name = params.name;
    this.breweryId = params.breweryId;
    this.beerStyleId = params.beerStyleId;
    this.minAlcoholByVolume = params.minAlcoholByVolume;
    this.maxAlcoholByVolume = params.maxAlcoholByVolume;
    this.minExtract = params.minExtract;
    this.maxExtract = params.maxExtract;
    this.minIbu = params.minIbu;
    this.maxIbu = params.maxIbu;
    this.minReleaseDate = params.minReleaseDate;
    this.maxReleaseDate = params.maxReleaseDate;
    this.minRating = params.minRating;
    this.maxRating = params.maxRating;
    this.minFavoritesCount = params.minFavoritesCount;
    this.maxFavoritesCount = params.maxFavoritesCount;
    this.minOpinionsCount = params.minOpinionsCount;
    this.maxOpinionsCount = params.maxOpinionsCount;
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
    if (this.breweryId && this.breweryId.trim() !== '') {
      params = params.append('breweryId', this.breweryId);
    }
    if (this.beerStyleId && this.beerStyleId.trim() !== '') {
      params = params.append('beerStyleId', this.beerStyleId);
    }
    if (this.minAlcoholByVolume) {
      params = params.append('minAlcoholByVolume', this.minAlcoholByVolume);
    }
    if (this.maxAlcoholByVolume) {
      params = params.append('maxAlcoholByVolume', this.maxAlcoholByVolume);
    }
    if (this.minExtract) {
      params = params.append('minExtract', this.minExtract);
    }
    if (this.maxExtract) {
      params = params.append('maxExtract', this.maxExtract);
    }
    if (this.minIbu) {
      params = params.append('minIbu', this.minIbu);
    }
    if (this.maxIbu) {
      params = params.append('maxIbu', this.maxIbu);
    }
    if (this.minReleaseDate && this.minReleaseDate.trim() !== '') {
      params = params.append('minReleaseDate', this.minReleaseDate);
    }
    if (this.maxReleaseDate && this.maxReleaseDate.trim() !== '') {
      params = params.append('maxReleaseDate', this.maxReleaseDate);
    }
    if (this.minRating) {
      params = params.append('minRating', this.minRating);
    }
    if (this.maxRating) {
      params = params.append('maxRating', this.maxRating);
    }
    if (this.minFavoritesCount) {
      params = params.append('minFavoritesCount', this.minFavoritesCount);
    }
    if (this.maxFavoritesCount) {
      params = params.append('maxFavoritesCount', this.maxFavoritesCount);
    }
    if (this.minOpinionsCount) {
      params = params.append('minOpinionsCount', this.minOpinionsCount);
    }
    if (this.maxOpinionsCount) {
      params = params.append('maxOpinionsCount', this.maxOpinionsCount);
    }

    return params;
  }

  static sortOptions = [
    {
      label: 'Release date (New to Old)',
      value: 'ReleaseDate',
      direction: 1
    },
    {
      label: 'Release date (Old to New)',
      value: 'ReleaseDate',
      direction: 0
    },
    { label: 'Name (A to Z)', value: 'Name', direction: 0 },
    { label: 'Name (Z to A)', value: 'Name', direction: 1 },
    { label: 'Beer style (A to Z)', value: 'BeerStyle', direction: 0 },
    { label: 'Beer style (Z to A)', value: 'BeerStyle', direction: 1 },
    {
      label: 'Alcohol by volume (Low to High)',
      value: 'AlcoholByVolume',
      direction: 0
    },
    {
      label: 'Alcohol by volume (High to Low)',
      value: 'AlcoholByVolume',
      direction: 1
    },
    { label: 'BLG (Low to High)', value: 'BLG', direction: 0 },
    { label: 'BLG (High to Low)', value: 'BLG', direction: 1 },
    { label: 'IBU (Low to High)', value: 'IBU', direction: 0 },
    { label: 'IBU (High to Low)', value: 'IBU', direction: 1 },
    { label: 'Rating (Low to High)', value: 'Rating', direction: 0 },
    { label: 'Rating (High to Low)', value: 'Rating', direction: 1 },
    {
      label: 'Opinions count (Low to High)',
      value: 'OpinionsCount',
      direction: 0
    },
    {
      label: 'Opinions count (High to Low)',
      value: 'OpinionsCount',
      direction: 1
    },
    {
      label: 'Favorites count (Low to High)',
      value: 'FavoritesCount',
      direction: 0
    },
    {
      label: 'Favorites count (High to Low)',
      value: 'FavoritesCount',
      direction: 1
    }
  ];
}
