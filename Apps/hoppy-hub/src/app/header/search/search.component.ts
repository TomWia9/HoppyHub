import { Component, inject, OnDestroy } from '@angular/core';
import { SearchService } from '../../services/search.service';
import { SearchResult } from '../../shared/search-result.model';
import { Subscription } from 'rxjs';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './search.component.html'
})
export class SearchComponent implements OnDestroy {
  private searchService: SearchService = inject(SearchService);
  private searchSubscription: Subscription = new Subscription();

  searchQuery: string = '';
  results: SearchResult | null = null;

  onSearch() {
    if (!this.searchQuery.trim()) {
      return;
    }

    this.searchSubscription = this.searchService
      .searchGlobal(this.searchQuery)
      .subscribe((data: SearchResult) => {
        this.results = data;
        console.log(this.results);
      });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }
}
