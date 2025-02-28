import { Component, inject, Input, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Beer } from '../../beers/beer.model';
import { Opinion } from '../opinion.model';
import { ModalService } from '../../services/modal.service';
import { ModalType } from '../../shared/model-type';
import { ModalModel } from '../../shared/modal-model';
import { BeersService } from '../../beers/beers.service';
import { Subscription } from 'rxjs';
import { ErrorMessageComponent } from '../../shared-components/error-message/error-message.component';
import { LoadingSpinnerComponent } from '../../shared-components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-opinion',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ErrorMessageComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './opinion.component.html'
})
export class OpinionComponent implements OnInit, OnDestroy {
  @Input({ required: true }) opinion!: Opinion;
  @Input() beer!: Beer;
  @Input() showBeerName: boolean = true;
  @Input() showUsername: boolean = true;
  @Input() editMode: boolean = false;

  private modalService: ModalService = inject(ModalService);
  private beersService: BeersService = inject(BeersService);
  private beerSubscription!: Subscription;

  error: string = '';
  loading: boolean = true;

  ngOnInit(): void {
    if (!this.beer) {
      this.beerSubscription = this.beersService
        .getBeerById(this.opinion.beerId)
        .subscribe({
          next: (beer: Beer) => {
            this.beer = beer;
            this.error = '';
            this.loading = false;
          },
          error: () => {
            this.error = 'An error occurred while loading the opinion';
            this.loading = false;
          }
        });
    } else {
      this.loading = false;
    }
  }

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

  ngOnDestroy(): void {
    if (this.beerSubscription) {
      this.beerSubscription.unsubscribe();
    }
  }
}
