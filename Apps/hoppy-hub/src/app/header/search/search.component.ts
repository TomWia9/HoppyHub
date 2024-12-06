import { Component, inject, OnDestroy } from '@angular/core';
import { SearchService } from '../../services/search.service';
import { SearchResult } from '../../shared/search-result.model';
import { Subscription } from 'rxjs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnDestroy {
  private searchService: SearchService = inject(SearchService);
  private searchSubscription: Subscription = new Subscription();

  searchQuery: string = '';
  results: SearchResult | null = null;
  isDropdownOpen = false;

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

  openDropdown() {
    this.isDropdownOpen = true;
  }

  closeDropdown() {
    this.isDropdownOpen = false;
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }
}
