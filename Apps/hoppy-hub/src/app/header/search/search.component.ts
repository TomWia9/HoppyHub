import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { SearchService } from '../../services/search.service';
import { SearchResult } from '../../shared/search-result.model';
import {
  debounceTime,
  distinctUntilChanged,
  filter,
  map,
  Subscription,
  switchMap,
  tap
} from 'rxjs';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faX } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, FontAwesomeModule],
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnInit, OnDestroy {
  private searchService: SearchService = inject(SearchService);
  private searchSubscription: Subscription = new Subscription();

  results: SearchResult | null = null;
  isDropdownOpen = false;
  faX = faX;
  searchForm = new FormControl<string>('');

  ngOnInit(): void {
    this.searchSubscription = this.searchForm.valueChanges
      .pipe(
        map(query => query || ''),
        debounceTime(400),
        distinctUntilChanged(),
        tap(query => {
          if (query.trim() === '') {
            this.results = null;
          }
        }),
        filter(query => query.trim() !== ''),
        switchMap(query => this.searchService.searchGlobal(query as string))
      )
      .subscribe(data => (this.results = data));
  }

  openDropdown(): void {
    this.isDropdownOpen = true;
  }

  closeDropdown(): void {
    this.isDropdownOpen = false;
  }

  onSearchClear(): void {
    this.searchForm.reset();
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }
}
