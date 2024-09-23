import {
  Component,
  ElementRef,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
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
import { AuthUser } from '../../../auth/auth-user.model';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { DeleteOpinionModalComponent } from '../../../opinions/delete-opinion-modal/delete-opinion-modal.component';
import { DataHelper } from '../../../shared/data-helper';

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
export class BeerOpinionsComponent
  extends DataHelper
  implements OnInit, OnChanges, OnDestroy
{
  @Input({ required: true }) beer!: Beer;
  @Input({ required: true }) user!: AuthUser | null;
  @Output() opinionsCountChanged = new EventEmitter<number>();
  @ViewChild('opinionsSection') opinionsSection!: ElementRef;

  private opinionsService: OpinionsService = inject(OpinionsService);
  private modalService: ModalService = inject(ModalService);

  sortOptions = OpinionsParams.sortOptions;

  opinionsParamsSubscription!: Subscription;
  getOpinionsSubscription!: Subscription;
  userSubscription!: Subscription;
  getUserOpinionsSubscription!: Subscription;
  selectedSortOptionIndex: number = 0;
  opinionsParams = new OpinionsParams({
    pageSize: 10,
    pageNumber: 1,
    sortBy: 'created',
    sortDirection: 1
  });
  opinions: PagedList<Opinion> | undefined;
  paginationData!: Pagination;
  error = '';
  opinionsLoading = true;
  existingOpinionloading = true;
  showOpinions = false;
  existingOpinion: Opinion | null = null;

  ngOnInit(): void {
    this.opinionsParamsSubscription =
      this.opinionsService.paramsChanged.subscribe((params: OpinionsParams) => {
        this.opinionsParams = params;
        this.opinionsParams.beerId = this.beer.id;
        this.getOpinions();
      });

    this.checkIfUserAlreadyAddedOpinion();
  }

  ngOnChanges(): void {
    this.refreshOpinions(0);
  }

  refreshOpinions(opinionsCountChange: number): void {
    this.opinionsParams.beerId = this.beer.id;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
    this.existingOpinion = null;
    this.opinionsCountChanged.emit(opinionsCountChange);
    this.checkIfUserAlreadyAddedOpinion();
  }

  onSort(): void {
    this.opinionsParams.pageNumber = 1;
    this.opinionsParams.sortBy =
      this.sortOptions[this.selectedSortOptionIndex].value;
    this.opinionsParams.sortDirection =
      this.sortOptions[this.selectedSortOptionIndex].direction;
    this.opinionsService.paramsChanged.next(this.opinionsParams);
  }

  onFiltersClear(): void {
    this.selectedSortOptionIndex = 0;
    this.opinionsParams = new OpinionsParams({
      pageSize: 10,
      pageNumber: 1,
      sortBy: 'created',
      sortDirection: 1
    });

    if (
      JSON.stringify(this.opinionsService.paramsChanged.value) !=
      JSON.stringify(this.opinionsParams)
    ) {
      this.opinionsService.paramsChanged.next(this.opinionsParams);
    }
  }

  toggleOpinions(): void {
    if (this.showOpinions) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
    this.showOpinions = !this.showOpinions;
  }

  scrollToTop(): void {
    const elementPosition =
      this.opinionsSection.nativeElement.getBoundingClientRect().top +
      window.scrollY;

    window.scrollTo({
      top: elementPosition,
      behavior: 'smooth'
    });
  }

  onUpsertOpinionModalOpen(): void {
    if (this.user) {
      this.modalService.openModal(ModalType.UpsertOpinion);
    } else {
      this.modalService.openModal(ModalType.Login);
    }
  }

  onDeleteOpinionModalOpen(): void {
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
          new OpinionsParams({
            pageSize: 1,
            pageNumber: 1,
            beerId: this.beer.id,
            userId: this.user.id
          })
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
          this.paginationData = this.getPaginationData(opinions);
          this.error = '';
          this.opinionsLoading = false;
        },
        error: error => {
          this.error = 'An error occurred while loading the opinions';

          if (error.error && error.error.errors) {
            const errorMessage = this.getValidationErrorMessage(
              error.error.errors
            );
            this.error += errorMessage;
          }

          this.opinionsLoading = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.getOpinionsSubscription) {
      this.getOpinionsSubscription.unsubscribe();
    }
    if (this.opinionsParamsSubscription) {
      this.opinionsParamsSubscription.unsubscribe();
    }
    if (this.getUserOpinionsSubscription) {
      this.getUserOpinionsSubscription.unsubscribe();
    }
  }
}
