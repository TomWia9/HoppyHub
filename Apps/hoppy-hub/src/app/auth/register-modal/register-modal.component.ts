import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  inject
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Subscription } from 'rxjs';
import { ModalService, ModalType } from '../../services/modal.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register-modal.component.html',
  styleUrl: './register-modal.component.css'
})
export class RegisterModalComponent implements OnInit, OnDestroy {
  private modalService = inject(ModalService);
  private router: Router = inject(Router);

  @ViewChild('registerModal') myModalRef!: ElementRef;
  modalOppenedSubscription!: Subscription;
  registerForm!: FormGroup;
  errorMessage = '';

  ngOnInit(): void {
    this.modalOppenedSubscription = this.modalService.modalOpened.subscribe(
      (modalType: ModalType) => {
        this.showModal(modalType);
      }
    );
    this.registerForm = new FormGroup({
      email: new FormControl('', [
        Validators.email,
        Validators.required,
        Validators.maxLength(256)
      ]),
      username: new FormControl('', [
        Validators.required,
        Validators.maxLength(256)
      ]),
      password: new FormControl('', [
        Validators.minLength(8),
        Validators.required,
        Validators.pattern('^(?=.*[^\\w])(?=.*\\d)(?=.*[A-Z]).+$')
      ])
    });
  }

  onFormReset() {
    this.errorMessage = '';
    this.registerForm.reset();
    if (this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).close();
    }
  }

  onSubmit() {
    if (this.registerForm.valid) {
      console.log('Sign up data:', this.registerForm.value);
      //authService.register();
      this.onFormReset();
      this.router.navigate(['/']);
    } else {
      console.log('The form is invalid');
    }
  }

  onSignIn() {
    this.onFormReset();
    this.modalService.openModal(ModalType.Login);
  }

  showModal(modalType: ModalType) {
    if (modalType === ModalType.Register && this.myModalRef) {
      (this.myModalRef.nativeElement as HTMLDialogElement).showModal();
    }
  }

  ngOnDestroy(): void {
    this.modalOppenedSubscription.unsubscribe();
  }
}
