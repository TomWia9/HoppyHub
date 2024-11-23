import { Component, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Beer } from '../../beers/beer.model';
import { Opinion } from '../opinion.model';
import { ModalService } from '../../services/modal.service';
import { UpsertOpinionModalComponent } from '../upsert-opinion-modal/upsert-opinion-modal.component';
import { ModalType } from '../../shared/model-type';
import { ModalModel } from '../../shared/modal-model';

@Component({
  selector: 'app-opinion',
  standalone: true,
  imports: [CommonModule, RouterModule, UpsertOpinionModalComponent],
  templateUrl: './opinion.component.html'
})
export class OpinionComponent {
  @Input({ required: true }) beer!: Beer;
  @Input({ required: true }) opinion!: Opinion;
  @Input() showBeerName: boolean = true;
  @Input() showUsername: boolean = true;
  @Input() editMode: boolean = false;

  private modalService: ModalService = inject(ModalService);

  getStars(rating: number): number[] {
    return Array.from({ length: rating }, (_, index) => index + 1);
  }

  getEmptyStars(rating: number): number[] {
    return Array(10 - rating).fill(0);
  }

  onUpsertOpinionModalOpen(): void {
    if (this.editMode) {
      this.modalService.openModal(
        new ModalModel(ModalType.UpsertOpinion, {
          beer: this.beer,
          opinion: this.opinion
        })
      );
    }
  }

  onDeleteOpinionModalOpen(): void {
    if (this.editMode) {
      this.modalService.openModal(
        new ModalModel(ModalType.DeleteOpinion, {
          opinionId: this.opinion.id
        })
      );
    }
  }
}
