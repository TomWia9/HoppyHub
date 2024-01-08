import { AbstractControl, ValidatorFn } from '@angular/forms';

export class CustomValidators {
  static passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: unknown } | null => {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');

      if (
        password &&
        confirmPassword &&
        password.value !== confirmPassword.value
      ) {
        return { passwordMismatch: true };
      }

      return null;
    };
  }

  static passwordPattern = '^(?=.*[^\\w])(?=.*\\d)(?=.*[A-Z]).+$';
}
