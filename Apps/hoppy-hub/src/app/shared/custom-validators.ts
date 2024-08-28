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

  static greaterThanOrEqualToControl(
    control1: string,
    control2: string
  ): ValidatorFn {
    return (control: AbstractControl): { [key: string]: unknown } | null => {
      const c1 = control.get(control1);
      const c2 = control.get(control2);

      if (c1 && c2 && c1.value && c2.value && c1.value < c2.value) {
        return { greaterThanOrEqualToControlError: true };
      }

      return null;
    };
  }

  static lessThanOrEqualToControl(
    control1: string,
    control2: string
  ): ValidatorFn {
    return (control: AbstractControl): { [key: string]: unknown } | null => {
      const c1 = control.get(control1);
      const c2 = control.get(control2);

      if (c1 && c2 && c1.value && c2.value && c1.value > c2.value) {
        return { lessThanOrEqualToControlError: true };
      }

      return null;
    };
  }
}
