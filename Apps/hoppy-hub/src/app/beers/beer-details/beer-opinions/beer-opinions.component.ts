import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnDestroy,
  Output,
  OnChanges
} from '@angular/core';
import { OpinionsService } from '../../../opinions/opinions.service';
import { OpinionsParams } from '../../../opinions/opinions-params';
import { Opinion } from '../../../opinions/opinion.model';
import { map, Subscription, take, tap } from 'rxjs';
import { Beer } from '../../beer.model';
import { OpinionComponent } from '../../../opinions/opinion/opinion.component';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../../../shared-components/pagination/pagination.component';
import { ModalService, ModalType } from '../../../services/modal.service';
import { UpsertOpinionModalComponent } from '../../../opinions/upsert-opinion-modal/upsert-opinion-modal.component';
import { AuthUser } from '../../../auth/auth-user.model';
import { LoadingSpinnerComponent } from '../../../shared-components/loading-spinner/loading-spinner.component';
import { DeleteOpinionModalComponent } from '../../../opinions/delete-opinion-modal/delete-opinion-modal.component';
import { BeerOpinionsListComponent } from './beer-opinions-list/beer-opinions-list.component';

@Component({
  selector: 'app-beer-opinions',
  standalone: true,
  imports: [
    OpinionComponent,
    FormsModule,
    PaginationComponent,
    UpsertOpinionModalComponent,
    LoadingSpinnerComponent,
    DeleteOpinionModalComponent,
    BeerOpinionsListComponent
  ],
  templateUrl: './beer-opinions.component.html'
})
export class BeerOpinionsComponent implements OnDestroy, OnChanges {
  @Input({ required: true }) beer!: Beer;
  @Input({ required: true }) user!: AuthUser | null;
  @Output() opinionsCountChanged = new EventEmitter<void>();

  private opinionsService: OpinionsService = inject(OpinionsService);
  private modalService: ModalService = inject(ModalService);

  getUserOpinionsSubscription!: Subscription;
  existingOpinionloading = true;
  existingOpinion: Opinion | null = null;

  ngOnChanges(): void {
    this.checkIfUserAlreadyAddedOpinion();
  }

  refreshOpinionsCount(): void {
    this.existingOpinion = null;
    this.opinionsCountChanged.emit();
    this.checkIfUserAlreadyAddedOpinion();
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

  ngOnDestroy(): void {
    if (this.getUserOpinionsSubscription) {
      this.getUserOpinionsSubscription.unsubscribe();
    }
  }
}
