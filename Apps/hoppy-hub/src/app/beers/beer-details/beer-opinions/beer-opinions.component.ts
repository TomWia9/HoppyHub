import {
  Component,
  ElementRef,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { OpinionsService } from '../../../opinions/opinions.service';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { Opinion } from '../../../opinions/opinion.model';
import { map, Subscription, take, tap } from 'rxjs';
import { PagedList } from '../../../shared/paged-list';
import { Pagination } from '../../../shared/pagination';
import { Beer } from '../../beer.model';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { ModalService, ModalType } from '../../../services/modal.service';
import { UpsertOpinionModalComponent } from '../../../opinions/upsert-opinion-modal/upsert-opinion-modal.component';
import { AuthService } from '../../../auth/auth.service';
import { AuthUser } from '../../../auth/auth-user.model';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { DeleteOpinionModalComponent } from '../../../opinions/delete-opinion-modal/delete-opinion-modal.component';

@Component({
  selector: 'app-beer-opinions',
  standalone: true,
  imports: [
    OpinionComponent,
    FormsModule,
    PaginationComponent,
    UpsertOpinionModalComponent,
    LoadingSpinnerComponent,
    DeleteOpinionModalComponent
  ],
  templateUrl: './beer-opinions.component.html'
})
export class BeerOpinionsComponent implements OnInit, OnChanges, OnDestroy {
  @Input({ required: true }) beer!: Beer;
  @ViewChild('opinionsSection') opinionsSection!: ElementRef;

  private opinionsService: OpinionsService = inject(OpinionsService);
  private modalService: ModalService = inject(ModalService);
  private authService: AuthService = inject(AuthService);

  sortOptions = [
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
    { label: 'Rating (High to Low)', value: 'Rating', direction: 1 },
    { label: 'Rating (Low to High)', value: 'Rating', direction: 0 }
  ];
  selectedSortOptionIndex: number = 0;
  opinionsParams = new OpinionsParams(10, 1, 'created', 1);
  opinions: PagedList<Opinion> | undefined;
  paginationData!: Pagination;
  error = '';
  opinionsLoading = true;
  existingOpinionloading = true;
  opinionsParamsSubscription!: Subscription;
  getOpinionsSubscription!: Subscription;
  userSubscription!: Subscription;
  getUserOpinionsSubscription!: Subscription;
  showOpinions = false;
  user: AuthUser | null | undefined;
  existingOpinion: Opinion | null = null;

  ngOnInit(): void {
    this.opinionsParamsSubscription =
      this.opinionsService.paramsChanged.subscribe((params: OpinionsParams) => {
        this.opinionsParams = params;
        this.opinionsParams.beerId = this.beer.id;
        this.getOpinions();
      });
    this.userSubscription = this.authService.user.subscribe(
      (user: AuthUser | null) => {
        this.user = user;
        this.checkIfUserAlreadyAddedOpinion();
      }
    );
  }

  ngOnChanges() {
    this.refreshOpinions();
  }

  refreshOpinions() {
    this.opinionsParams.beerId = this.beer.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
    this.existingOpinion = null;
    this.checkIfUserAlreadyAddedOpinion();
  }

  onSort() {
    this.opinionsParams.pageNumber = 1;
    this.opinionsParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.opinionsParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onFiltersClear() {
    this.selectedSortOptionIndex = 0;
    this.opinionsParams = new OpinionsParams(10, 1, 'created', 1);

    if (
      JSON.stringify(this.opinionsService.paramsChanged.value) !=
      JSON.stringify(this.opinionsParams)
    ) {
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  toggleOpinions() {
    if (this.showOpinions) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
    this.showOpinions = !this.showOpinions;
  }

  scrollToTop() {
    const elementPosition =
      this.opinionsSection.nativeElement.getBoundingClientRect().top +
      window.scrollY;

    window.scrollTo({
      top: elementPosition,
      behavior: 'smooth'
    });
  }

  onUpsertOpinionModalOpen() {
    if (this.user) {
      this.modalService.openModal(ModalType.UpsertOpinion);
    } else {
      this.modalService.openModal(ModalType.Login);
    }
  }

  onDeleteOpinionModalOpen() {
    if (this.user) {
      this.modalService.openModal(ModalType.DeleteOpinion);
    }
  }

  private checkIfUserAlreadyAddedOpinion(): void {
    this.existingOpinionloading = true;
    if (!this.user) {
      this.existingOpinion = null;
      this.existingOpinionloading = false;
    } else {
      this.getUserOpinionsSubscription = this.opinionsService
        .getOpinions(
          new OpinionsParams(
            1,
            1,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            this.beer.id,
            this.user.id
          )
        )
        .pipe(
          take(1),
          map(opinions => (opinions.TotalCount > 0 ? opinions.items[0] : null)),
          tap(opinion => {
            this.existingOpinion = opinion;
            this.existingOpinionloading = false;
          })
        )
        .subscribe();
    }
  }

  private getOpinions(): void {
    this.getOpinionsSubscription = this.opinionsService
      .getOpinions(this.opinionsParams)
      .subscribe({
        next: (opinions: PagedList<Opinion>) => {
          this.opinionsLoading = true;
          this.opinions = opinions;
          this.paginationData = this.getPaginationData();
          this.error = '';
          this.opinionsLoading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the opinions';

          if (error.error && error.error.errors) {
            const errorMessage = this.getErrorMessage(error.error.errors);
            this.error += errorMessage;
          }

          this.opinionsLoading = false;
        }
      });
  }

  private getPaginationData(): Pagination {
    if (this.opinions) {
      return {
        CurrentPage: this.opinions.CurrentPage,
        HasNext: this.opinions.HasNext,
        HasPrevious: this.opinions.HasPrevious,
        TotalPages: this.opinions.TotalPages,
        TotalCount: this.opinions.TotalCount
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

  private getErrorMessage(array: { [key: string]: string }[]): string {
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

  ngOnDestroy(): void {
    this.getOpinionsSubscription.unsubscribe();
    this.opinionsParamsSubscription.unsubscribe();
    this.userSubscription.unsubscribe();
    this.getUserOpinionsSubscription.unsubscribe();
  }
}
