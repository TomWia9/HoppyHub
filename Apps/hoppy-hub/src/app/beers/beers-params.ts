import { HttpParams } from '@angular/common/http';
import { Params } from '@angular/router';

export class BeersParams implements Params {
  constructor(
    public pageSize: number,
    public pageNumber: number,
    public sortBy?: string,
    public sortDirection?: number,
    public searchQuery?: string,
    public name?: string,
    public breweryId?: string,
    public beerStyleId?: string,
    public minAlcoholByVolume?: number,
    public maxAlcoholByVolume?: number,
    public minExtract?: number,
    public maxExtract?: number,
    public minIbu?: number,
    public maxIbu?: number,
    public minReleaseDate?: string,
    public maxReleaseDate?: string,
    public minRating?: number,
    public maxRating?: number,
    public minFavoritesCount?: number,
    public maxFavoritesCount?: number,
    public minOpinionsCount?: number,
    public maxOpinionsCount?: number
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
}
