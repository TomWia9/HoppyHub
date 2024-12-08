import { Component, inject, OnDestroy } from '@angular/core';
import { SearchService } from '../../services/search.service';
import { SearchResult } from '../../shared/search-result.model';
import { Subscription } from 'rxjs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faX } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, RouterModule, FontAwesomeModule],
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnDestroy {
  private searchService: SearchService = inject(SearchService);
  private searchSubscription: Subscription = new Subscription();

  searchQuery: string = '';
  results: SearchResult | null = null;
  isDropdownOpen = false;
  faX = faX;

  onSearch() {
    if (!this.searchQuery.trim()) {
      this.results = null;
      return;
    }

    this.searchSubscription = this.searchService
      .searchGlobal(this.searchQuery)
      .subscribe((data: SearchResult) => {
        this.results = data;
      });
  }

  openDropdown(): void {
    this.isDropdownOpen = true;
  }

  closeDropdown(): void {
    this.isDropdownOpen = false;
  }

  onSearchClear(): void {
    this.searchQuery = '';
    this.onSearch();
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }
}
